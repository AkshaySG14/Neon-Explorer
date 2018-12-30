using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectileConsts;

public class FastPlayerMirror : PlayerMirror
{
    private Vector2 endVector;

    FastPlayerMirror()
    {
        shootForce = Constants.FAST_MIRROR_SHOOT_FORCE;
        lifeSpan = Constants.FAST_MIRROR_LIFESPAN;
    }

    public override void SetPathVector(Vector2 endVector)
    {
        this.endVector = endVector;
        this.rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine(Launcher());
        StartCoroutine("SlowDown");
        StartCoroutine("SelfDestruct");
    }

    void OnCollisionEnter2D(Collision2D collisionObject)
    {
        Explode();
    }

    private IEnumerator SlowDown()
    {
        while (Mathf.Abs(endVector.magnitude - transform.position.magnitude) > 0.01f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            rb2d.velocity = new Vector2(endVector.x - transform.position.x, endVector.y - transform.position.y) * shootForce;
            rb2d.position = Vector2.Lerp(transform.position, endVector, 1 / 10.0f);
        }
        yield return null;
        rb2d.velocity = Vector2.zero;
    }

    private IEnumerator Launcher()
    {
        yield return 0;
        yield return 0;
        spawning = false;
        this.GetComponent<Renderer>().enabled = true;
    }
}
