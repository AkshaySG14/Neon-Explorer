using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowPlayerMirror : MonoBehaviour
{
    public GameObject explosionPrefab;

    private Rigidbody2D rb2d;

    private static string ENEMY_TAG = "Enemy";

    private const float SHOOT_FORCE = 0.5f;

    // Use this for initialization
    void Start()
    {
        this.rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 2.5f);
    }

    public void SetPathVector(Vector2 pathVector)
    {
        this.rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine("Launch", pathVector);
        StartCoroutine("SelfDestruct");
    }

    void OnCollisionEnter2D(Collision2D collisionObject)
    {
        var objectTag = collisionObject.gameObject.tag;

        if (objectTag.Equals(ENEMY_TAG))
        {
            collisionObject.gameObject.SendMessage(
                "TakeDamage",
                1,
                SendMessageOptions.DontRequireReceiver
            );
        }
        Explode();
    }

    private void Launch(Vector2 pathVector)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        rb2d.velocity = (pathVector - position) * SHOOT_FORCE;
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5f);
        Explode();
    }

    private void Explode()
    {
        Instantiate(explosionPrefab, gameObject.transform.position,
                    gameObject.transform.rotation);
        Destroy(this.gameObject);
    }
}
