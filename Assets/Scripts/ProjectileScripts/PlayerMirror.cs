using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public GameObject explosionPrefab;

    private Rigidbody2D rb2d;

    private static string PLAYER_TAG = "Player";
    private static string ENEMY_TAG = "Enemy";

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine("SlowDown");
        StartCoroutine("SelfDestruct");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collisionObject)
    {
        var objectTag = collisionObject.gameObject.tag;

        if (objectTag.Equals(PLAYER_TAG))
        {
            Physics2D.IgnoreCollision(
                collisionObject.gameObject.GetComponent<BoxCollider2D>(),
                gameObject.GetComponent<BoxCollider2D>()
            );
            return;
        }

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

    private IEnumerator SlowDown()
    {
        yield return null;
        Vector2 sVelocity = rb2d.velocity;
        sVelocity.Scale(new Vector2(-5f, -5f));

        int counter = 0;
        while (counter < 10)
        {
            rb2d.AddForce(sVelocity);
            counter++;
            yield return new WaitForSeconds(Time.deltaTime / 10);
        }

        yield return null;
        rb2d.velocity.Set(0, 0);
    }


    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(3f);
        Explode();
    }

    private void Explode()
    {
        Instantiate(explosionPrefab, gameObject.transform.position,
                    gameObject.transform.rotation);
        Destroy(this.gameObject);
    }
}
