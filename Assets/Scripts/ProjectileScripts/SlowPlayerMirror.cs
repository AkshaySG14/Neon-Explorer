using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectileConsts;

public class SlowPlayerMirror : PlayerMirror
{
    SlowPlayerMirror()
    {
        shootForce = Constants.SLOW_MIRROR_SHOOT_FORCE;
        lifeSpan = Constants.SLOW_MIRROR_LIFESPAN;
    }

    public override void SetPathVector(Vector2 pathVector)
    {
        this.rb2d = GetComponent<Rigidbody2D>();
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        rb2d.velocity = (pathVector - position) * shootForce;
        StartCoroutine(Launcher());
        StartCoroutine("SelfDestruct");
    }

    private IEnumerator Launcher()
    {
        yield return 0;
        yield return 0;
        spawning = false;
        this.GetComponent<Renderer>().enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (spawning)
        {
            Explode();
        }
    }
}
