using System.Collections;
using UnityEngine;
using Player;

public class PlayerController : MonoBehaviour
{

    public Transform groundCheck;
    public Transform projectile;

    public Camera camera;
    public GameObject fastMirror;
    public GameObject slowMirror;

    private int health = 5;
    private float jumpTime = 0.1f;

    private bool facingRight = true;
    private bool grounded = false;
    private bool stunned = false;
    private bool invulnerable = false;
    private bool crouched = false;
    private bool dJumped = false;
    private bool running = false;

    private int groundTimer = 0;
    private bool isLandingFrame = false;


    private Animator animator;
    private Rigidbody2D rb2d;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Interrupt()
    {
        StopCoroutine("Slide");
        running = false;
        rb2d.velocity *= 0;
        enabled = false;
    }

    private void Update()
    {
        if (groundTimer == 0)
        {
            // Checks to see if there is something separating the player and the 
            // ground check transform.
            Debug.DrawRay(groundCheck.position, Vector2.down, Color.green);
            bool newGroundedState = Physics2D.Raycast(
                groundCheck.position,
                Vector2.down,
                0.25f,
                1 << LayerMask.NameToLayer(Constants.BLOCKING_LAYER)
            );
            if (!grounded) {
                if (newGroundedState)
                {
                    AnimTrigger(Constants.GROUNDED);
                    animator.SetBool(Constants.AIR, false);
                    groundTimer = Constants.GROUND_CHECK_TIMER;
                    isLandingFrame = true;
                    Land(rb2d.velocity.x);
                }
                else
                {
                    animator.SetBool(Constants.AIR, true);
                }
            }
            grounded = newGroundedState;
        } else groundTimer--;
        CheckInput();
        isLandingFrame = false;
    }

    private void CheckInput()
    {
        if (stunned)
        {
            return;
        }
        if (Input.GetButtonDown(Constants.CROUCH) && grounded)
        {
            Crouch(true);
            return;
        }
        else if (Input.GetButtonUp(Constants.CROUCH) || !grounded)
        {
            Crouch(false);
        }

        if (isLandingFrame)
        {
            if (Input.GetButton(Constants.CROUCH))
            {
                Crouch(true);
                return;
            }
        }
        else
        {
            float h = Input.GetAxis(Constants.HORIZONTAL);
            // Plays idle and halts player if no horizontal input and not crouched.
            if (Mathf.Approximately(h, 0) && !crouched && grounded)
            {
                ChangeVel(0, rb2d.velocity.y);
                AnimTrigger(Constants.IDLE);
            }
            else if (!crouched)
            {
                Move(h);
            }
            CheckFlip(h);
        }

        
        if (Input.GetButtonDown(Constants.SHOOT_FAST_MIRROR))
        {
            FireMirror(fastMirror);
        }
        else if (Input.GetButtonDown(Constants.SHOOT_SLOW_MIRROR))
        {
            FireMirror(slowMirror);
        }
        if (Input.GetButtonDown(Constants.JUMP) && grounded)
        {
            StartCoroutine(Jump());
        }
        else if (Input.GetButtonUp(Constants.JUMP) && rb2d.velocity.y > 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0.5f);
        }
        if (Input.GetButtonDown(Constants.JUMP) && !grounded && !dJumped)
        {
            print(Input.GetAxis(Constants.VERTICAL));
            //Djump Down
            if (Input.GetAxisRaw(Constants.VERTICAL) < 0)
            {
                ChangeVel(0, Constants.DJUMP_DOWN_VEL);
            }
            else if (Input.GetAxis(Constants.HORIZONTAL) != 0)
            {
                //Minimum Side Dodge Velocity
                if(Mathf.Abs(rb2d.velocity.x) < Constants.DJUMP_SIDE_MIN_VEL) {
                    ChangeVel(Constants.DJUMP_SIDE_MIN_VEL * Mathf.Sign(rb2d.velocity.x),
                        rb2d.velocity.y);
                }
                //Djump Right
                if (Input.GetAxis(Constants.HORIZONTAL) > 0)
                {
                    ChangeVel(Mathf.Abs(rb2d.velocity.x * Constants.DJUMP_SIDE_VEL_MULTIPLIER), 
                        Constants.DJUMP_SIDE_VER_VEL);
                }
                //Djump Left
                else
                {
                    ChangeVel(-Mathf.Abs(rb2d.velocity.x * Constants.DJUMP_SIDE_VEL_MULTIPLIER), 
                        Constants.DJUMP_SIDE_VER_VEL);
                }
            }
            //Djump up
            else
            {
                ChangeVel(rb2d.velocity.x * Constants.DJUMP_UP_SIDE_VEL_MULTIPLIER, 
                    Constants.DJUMP_UP_VEL);
            }
            
            rb2d.gravityScale = 1.5f;
            dJumped = true;
        }
    }


    private void FixedUpdate()
    {
        if (stunned)
        {
            return;
        }
     

       
    }

    private void ChangeVel(float x, float y)
    {
        rb2d.velocity = new Vector2(x, y);
    }

    private void CheckFlip(float h)
    {
        if ((h > 0 && !facingRight) || (h < 0 && facingRight))
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void Move(float h)
    {
        if (crouched)
        {
            return;
        }

        AnimTrigger(Constants.MOVEMENT);

        animator.SetFloat(Constants.SPEED, Mathf.Abs(h));
        if (h * rb2d.velocity.x < Constants.MAX_SPEED)
        {
            if (grounded)
            {
                rb2d.AddForce(Vector2.right *
                              h *
                              Constants.MOVE_FORCE *
                              Constants.GROUND_SPEED_MULTIPLIER);
            }
            else
            {
                rb2d.AddForce(Vector2.right *
                              h *
                              Constants.MOVE_FORCE *
                              Constants.AIR_SPEED_MULTIPLIER);
            }
        }
        // Checks for exceeding max speed.
        if (Mathf.Abs(rb2d.velocity.x) > Constants.MAX_SPEED && !dJumped)
        {
            ChangeVel(Mathf.Sign(rb2d.velocity.x) *
                                        Constants.MAX_SPEED, rb2d.velocity.y);
        }
    }

    private void FireMirror(GameObject gameObject)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = camera.ScreenToWorldPoint(mousePos);

        if (mousePos.x < transform.position.x && facingRight)
        {
            Flip();
        }
        else if (mousePos.x > transform.position.x && !facingRight)
        {
            Flip();
        }

        if (grounded)
        {
            AnimTrigger(Constants.SHOOT);
            StopCoroutine("ShootEnd");
            StartCoroutine("ShootEnd");
        }

        GameObject mirrorProj;
        mirrorProj = Instantiate(gameObject, projectile.position, projectile.rotation);

        mirrorProj.SendMessage("SetPathVector", new Vector2(mousePos.x, mousePos.y));
    }

    private void FireSlowMirror()
    {
        if (grounded)
        {
            AnimTrigger(Constants.SHOOT);
            StopCoroutine("ShootEnd");
            StartCoroutine("ShootEnd");
        }

        GameObject mirrorProj;
        mirrorProj = Instantiate(slowMirror, projectile.position, projectile.rotation);

        Vector3 mousePos = Input.mousePosition;
        mousePos = camera.ScreenToWorldPoint(mousePos);
        if (mousePos.x < transform.position.x && facingRight)
        {
            Flip();
        }
        else if (mousePos.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        mirrorProj.SendMessage("SetPathVector", new Vector2(mousePos.x, mousePos.y));
    }


    private void TakeDamage(int damage)
    {
        if (invulnerable)
        {
            return;
        }

        stunned = true;
        invulnerable = true;

        health -= damage;

        ChangeVel(0, rb2d.velocity.y);

        StartCoroutine("UnStun", 0.1);
        StartCoroutine("EndInvulnerability", 0.4f);
        StartCoroutine("Flash");
    }

    private void Land(float velX)
    {
        dJumped = false;
        rb2d.gravityScale = 1;
        StartCoroutine(Slide(velX));
    }

    private void Crouch(bool state)
    {
        if (state)
        {
            crouched = true;
            rb2d.drag = Constants.CROUCH_DRAG;
            AnimTrigger(Constants.CROUCH);
        }
        else
        {
            crouched = false;
            AnimTrigger(Constants.IDLE);
            rb2d.drag = 0;
        }
    }

    private void AnimTrigger(string triggerName)
    {
        foreach (AnimatorControllerParameter p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(p.name);
        animator.SetTrigger(triggerName);
    }


    private IEnumerator Slide(float velX)
    {
        yield return null;
        float time = 0;
        float currVelX = velX*Constants.SLIDE_INITIAL_MULTIPLIER;
        float sign = Mathf.Sign(velX);
        while (time < Constants.SLIDE_TIMER && grounded && Input.GetAxis(Constants.HORIZONTAL) == 0)
        {
            yield return null;
            currVelX *= Constants.SLIDE_DRAG;
             rb2d.AddForceAtPosition(new Vector2(currVelX*Constants.SLIDE_FORCE* Time.deltaTime, 0), rb2d.position);
            time += Time.deltaTime;
            
        }
    }

    private IEnumerator Jump()
    {
        crouched = false;
        StartCoroutine("setUnGrounded");
        AnimTrigger(Constants.JUMP);

        ChangeVel(rb2d.velocity.x, Constants.JUMP_SPEED);
        yield return null;

        float time = 0;

        while (Input.GetButton(Constants.JUMP) && time < jumpTime)
        {
            ChangeVel(rb2d.velocity.x, Constants.JUMP_SPEED);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator setUnGrounded()
    {
        yield return new WaitForSeconds(0.35f);
        grounded = false;
    }

    private IEnumerator Flash()
    {
        float flash = 0;
        while (flash < 1)
        {
            flash += 0.05f;
            GetComponent<SpriteRenderer>().material.SetFloat("_FlashAmount", flash);
            yield return new WaitForSeconds(Time.deltaTime / 10);
        }
        while (flash > 0)
        {
            flash -= 0.05f;
            GetComponent<SpriteRenderer>().material.SetFloat("_FlashAmount", flash);
            yield return new WaitForSeconds(Time.deltaTime / 10);
        }
        GetComponent<SpriteRenderer>().material.SetFloat("_FlashAmount", 0);
    }

    private IEnumerator UnStun(float deltaTime)
    {
        yield return new WaitForSeconds(deltaTime);
        stunned = false;
    }

    private IEnumerator EndInvulnerability(float deltaTime)
    {
        yield return new WaitForSeconds(deltaTime);
        invulnerable = false;
    }

    private IEnumerator ShootEnd()
    {
        yield return new WaitForSeconds(0.2f);
        AnimTrigger(Constants.SHOOTSTOP);
    }
}
