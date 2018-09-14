using System.Collections;
using UnityEngine;
using Enemy;

public abstract class EnemyScript : MonoBehaviour
{
    public Transform groundCheck;
    public GameObject explosionPrefab;

    private int health = 0;

    private bool facingRight = true;
    private bool grounded = false;
    private bool stunned = false;
    private bool invulnerable = false;

    protected Rigidbody2D rb2d;

    private Animator animator;

    protected virtual void Start(int health)
    {
        this.health = health;
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        // Checks to see if there is something separating the player and the 
        // ground check transform.
        grounded = Physics2D.OverlapCircle(
            groundCheck.position,
            Constants.GROUND_RADIUS,
            1 << LayerMask.NameToLayer(Constants.BLOCKING_LAYER)
        );
    }

    protected virtual void Move(float h)
    {
        if (stunned)
        {
            return;
        }

        if (Mathf.Approximately(h, 0))
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            AnimTrigger(Constants.IDLE);
        }

        else
        {
            AnimTrigger(Constants.MOVEMENT);

            animator.SetFloat(Constants.SPEED, Mathf.Abs(h));

            rb2d.velocity = new Vector2(h, rb2d.velocity.y);
        }
    }

    protected void CheckFlip(float h)
    {
        if ((h > 0 && !facingRight) || (h < 0 && facingRight))
        {
            Flip();
        }
    }

    protected void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    protected virtual void TakeDamage(int damage)
    {
        if (invulnerable)
        {
            return;
        }

        invulnerable = true;

        DecreaseHealth(damage);

        if (health > 0)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);

            StartCoroutine("EndInvulnerability", 0.2f);
            StartCoroutine("Flash");
        }
        else
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Instantiate(explosionPrefab, gameObject.transform.position,
            gameObject.transform.rotation);
        Destroy(this.gameObject);
    }

    protected void AnimTrigger(string triggerName)
    {
        foreach (AnimatorControllerParameter p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(p.name);
        animator.SetTrigger(triggerName);
    }

    protected bool IsGrounded()
    {
        return grounded;
    }

    protected bool IsStunned()
    {
        return stunned;
    }

    protected void SetHealth(int health)
    {
        this.health = health;
    }

    protected void DecreaseHealth(int amount)
    {
        health -= amount;
    }

    protected int GetHealth()
    {
        return health;
    }

    protected virtual IEnumerator Flash()
    {
        float flash = 0;
        while (flash < 1)
        {
            flash += 0.2f;
            GetComponent<SpriteRenderer>().material.SetFloat("_FlashAmount", flash);
            yield return new WaitForSeconds(Time.deltaTime / 10);
        }
        while (flash > 0)
        {
            flash -= 0.2f;
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
}
