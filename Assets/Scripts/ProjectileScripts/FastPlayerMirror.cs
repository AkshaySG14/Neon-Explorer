using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectileConsts;

public class FastPlayerMirror : PlayerMirror
{
    private Vector2 endVector;
    private FastPlayerMirror primaryMirror = null;
    private FastPlayerMirror secondaryMirror = null;
    private LineRenderer mirrorLineRenderer;
    private EdgeCollider2D mirrorCollider;

    FastPlayerMirror()
    {
        shootForce = Constants.FAST_MIRROR_SHOOT_FORCE;
        lifeSpan = Constants.FAST_MIRROR_LIFESPAN;
    }

    public override void SetPathVector(Vector2 endVector)
    {
        this.endVector = endVector;
        this.rb2d = GetComponent<Rigidbody2D>();
        this.mirrorCollider = GetComponent<EdgeCollider2D>();
        StartCoroutine(Launcher());
        StartCoroutine("SlowDown");
        StartCoroutine("SelfDestruct");
    }

    void OnCollisionEnter2D(Collision2D collisionObject)
    {
        Explode();
    }

    protected override void Explode()
    {
        if (primaryMirror != null)
        {
            primaryMirror.Explode();
        }
        Instantiate(explosionPrefab, gameObject.transform.position,
            gameObject.transform.rotation);
        Destroy(this.gameObject);
    }

    void SetPrimaryMirror(FastPlayerMirror primaryMirror)
    {
        if (primaryMirror == null)
        {
            return;
        }
        else
        {
            this.mirrorLineRenderer = GetComponent<LineRenderer>();
            this.primaryMirror = primaryMirror;
        }
    }

    void SetSecondaryMirror(FastPlayerMirror secondaryMirror)
    {
        if (secondaryMirror == null)
        {
            return;
        }
        else
        {
            this.mirrorLineRenderer = GetComponent<LineRenderer>();
            this.secondaryMirror = secondaryMirror;
            secondaryMirror.SetPrimaryMirror(this);
        }
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
        if (secondaryMirror != null)
        {
            mirrorLineRenderer.SetPosition(0, this.transform.position);
            mirrorLineRenderer.SetPosition(1, secondaryMirror.transform.position);
            mirrorCollider.points = new Vector2[] { new Vector2(0, 0), secondaryMirror.transform.position - transform.position };
        }
    }

    private IEnumerator Launcher()
    {
        yield return 0;
        yield return 0;
        spawning = false;
        this.GetComponent<Renderer>().enabled = true;
    }
}
