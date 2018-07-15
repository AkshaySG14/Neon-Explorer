using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {

    public Transform groundCheck;
    public Transform projectile;

    public GameObject bulletPrefab;

    private bool facingRight = true;

    private const float MOVE_FORCE = 365f;
    private const float MAX_SPEED = 3f;
    private const float SPEEDMULTIPLIER = 0.2f;
    private const float JUMPFORCE = 135f;
    private const float GROUNDRADIUS = 0.5f;
    private const float JUMPDECAYRATE = 0.33f;

    private const float SHOOT_FORCE = 1000f;

    private const string IDLE = "Idle";
    private const string GROUNDED = "Grounded";
    private const string MOVEMENT = "Movement";
    private const string JUMP = "Jump";
    private const string CROUCH = "Crouch";
    private const string SHOOT = "Shoot";
    private const string SHOOTSTOP = "ShootStop";

    private const string HORIZONTAL = "Horizontal";
    private const string SPEED = "Speed";

    private const string BLOCKINGLAYER = "Blocking";

    private bool grounded = false;
    private bool crouched = false;
    private Animator animator;
    private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        // Checks to see if there is something separating the player and the 
        // ground check transform.
        bool newGroundedState = Physics2D.OverlapCircle(
            groundCheck.position, 
            GROUNDRADIUS, 
            1 << LayerMask.NameToLayer(BLOCKINGLAYER)
        );
        // If the player was in the air, and is now grounded, reset jump anim.
        if (!grounded && newGroundedState) {
            AnimTrigger(GROUNDED);
        }
        grounded = newGroundedState;
        CheckInput();
	}

    private void CheckInput() {
        if (Input.GetButtonDown(CROUCH) && grounded) {
            crouched = true;
            AnimTrigger(CROUCH);
        }
        if (Input.GetButtonUp(CROUCH) && grounded) {
            crouched = false;
            AnimTrigger(IDLE);
        }
        if (Input.GetButtonDown(SHOOT)) {
            Shoot();
        }
        if (Input.GetButtonDown(JUMP) && grounded && !crouched) {
            StartCoroutine(Jump());
        }
    }

    private void FixedUpdate() {
        float h = Input.GetAxis(HORIZONTAL);

        // Plays idle and halts player if no horizontal input and not crouched.
        if (Mathf.Approximately(h, 0) && !crouched) {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            AnimTrigger(IDLE);
        }
        else {
            // Flips sprite accordingly.
            if (h > 0 && !facingRight)
            {
                Flip();
            }
            else if (h < 0 && facingRight)
            {
                Flip();
            }

            // If crouched, player cannot move. 
            if (crouched) {
                return;
            }

            AnimTrigger(MOVEMENT);

            animator.SetFloat(SPEED, Mathf.Abs(h));

            if (h * rb2d.velocity.x < MAX_SPEED) {
                rb2d.AddForce(Vector2.right * h * MOVE_FORCE * SPEEDMULTIPLIER);
            }
            // Checks for exceeding max speed.
            if (Mathf.Abs(rb2d.velocity.x) > MAX_SPEED) {
                rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * 
                                            MAX_SPEED, rb2d.velocity.y);
            }
        }
    }

    private void Shoot() {
        if (grounded) {
            AnimTrigger(SHOOT);
            StopCoroutine("ShootEnd");
            StartCoroutine("ShootEnd");
        }

        GameObject bullet;
        bullet = Instantiate(bulletPrefab, projectile.position, projectile.rotation);

        bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.forward * shootForce);
    }

    private IEnumerator Jump() {
        AnimTrigger(JUMP);
        float force = JUMPFORCE;

        rb2d.AddForce(Vector2.up * force);

        while (Input.GetButton(JUMP) && grounded) {
            rb2d.AddForce(Vector2.up * force * JUMPDECAYRATE);
            yield return null;
        }
    }

    private IEnumerator ShootEnd() {
        yield return new WaitForSeconds(0.5f);
        AnimTrigger(SHOOTSTOP);
    }

    private void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    } 

    void AnimTrigger(string triggerName)
    {
        foreach (AnimatorControllerParameter p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(p.name);
        animator.SetTrigger(triggerName);
    }
}
 