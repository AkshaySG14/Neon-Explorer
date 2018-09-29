using System.Collections;
using UnityEngine;
using Player;

public class PlayerController : MonoBehaviour
{

    public Transform groundCheck;
    public Transform projectile;

    public Camera camera;
    public GameObject bulletPrefab;

    private int health = 5;
    private float jumpTime = 0.1f;

    private bool facingRight = true;
    private bool grounded = false;
    private bool stunned = false;
    private bool invulnerable = false;
    private bool crouched = false;

    private Animator animator;
    private Rigidbody2D rb2d;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
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
        if (!grounded && newGroundedState)
        {
            AnimTrigger(Constants.GROUNDED);
        }
        grounded = newGroundedState;
        CheckInput();
    }

    private void CheckInput()
    {
        if (stunned)
        {
            return;
        }
        else if (Input.GetButtonDown(Constants.CROUCH) && grounded &&
            Mathf.Approximately(rb2d.velocity.x, 0))
        {
            crouched = true;
            AnimTrigger(Constants.CROUCH);
        }
        else if (Input.GetButtonUp(Constants.CROUCH) && grounded)
        {
            crouched = false;
            AnimTrigger(Constants.IDLE);
        }
        else if (Input.GetButtonDown(Constants.SHOOT))
        {
            Fire();
        }
        else if (Input.GetButtonDown(Constants.JUMP) && grounded && !crouched)
        {
            StartCoroutine(Jump());
        }
        else if (Input.GetButtonUp(Constants.JUMP) && rb2d.velocity.y > 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0.5f);
        }
    }

    private void FixedUpdate()
    {
        if (stunned)
        {
            return;
        }

        float h = Input.GetAxis(Constants.HORIZONTAL);

        // Plays idle and halts player if no horizontal input and not crouched.
        if (Mathf.Approximately(h, 0) && !crouched && grounded)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            AnimTrigger(Constants.IDLE);
        }
        else
        {
            CheckFlip(h);
            Move(h);
        }
    }

    private void CheckFlip(float h)
    {
        if ((h > 0 && !facingRight) || (h < 0 && facingRight))
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
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
        if (Mathf.Abs(rb2d.velocity.x) > Constants.MAX_SPEED)
        {
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) *
                                        Constants.MAX_SPEED, rb2d.velocity.y);
        }
    }

    private void Shoot()
    {
        if (grounded)
        {
            AnimTrigger(Constants.SHOOT);
            StopCoroutine("ShootEnd");
            StartCoroutine("ShootEnd");
        }

        GameObject bullet;
        bullet = Instantiate(bulletPrefab, projectile.position,
                             projectile.rotation);
        bullet.GetComponent<Rigidbody2D>().AddForce(
            new Vector2(Mathf.Sign(transform.localScale.x), 0) *
            Constants.SHOOT_FORCE
        );
    }

    private void Fire()
    {
        if (grounded)
        {
            AnimTrigger(Constants.SHOOT);
            StopCoroutine("ShootEnd");
            StartCoroutine("ShootEnd");
        }

        GameObject mirror;
        mirror = Instantiate(bulletPrefab, projectile.position,
                             projectile.rotation);

        Vector3 mousePos = Input.mousePosition;
        mousePos = camera.ScreenToWorldPoint(mousePos);
        float mousePosX = mousePos.x - transform.position.x;
        float mousePosY = mousePos.y - transform.position.y;

        Debug.Log(new Vector2(mousePosX, mousePosY) * Constants.SHOOT_FORCE);

        mirror.GetComponent<Rigidbody2D>().AddForce(
            new Vector2(mousePosX, mousePosY) * Constants.SHOOT_FORCE
        );
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

        rb2d.velocity = new Vector2(0, rb2d.velocity.y);

        StartCoroutine("UnStun", 0.1);
        StartCoroutine("EndInvulnerability", 0.4f);
        StartCoroutine("Flash");
    }

    private void AnimTrigger(string triggerName)
    {
        foreach (AnimatorControllerParameter p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(p.name);
        animator.SetTrigger(triggerName);
    }

    private IEnumerator Jump()
    {
        StartCoroutine("setUnGrounded");
        AnimTrigger(Constants.JUMP);

        rb2d.velocity = new Vector2(rb2d.velocity.x, Constants.JUMP_SPEED);
        yield return null;

        float time = 0;

        while (Input.GetButton(Constants.JUMP) && time < jumpTime)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, Constants.JUMP_SPEED);
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
