using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public GameObject explosionPrefab;

    private Rigidbody2D rb2d;

    private static string ENEMY_TAG = "Enemy";

    private const float SHOOT_FORCE = 5f;

    private Vector2 endVector;

    // Use this for initialization
    void Start()
    {
        this.rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetEndVector(Vector2 endVector)
    {
        this.rb2d = GetComponent<Rigidbody2D>();
        this.endVector = endVector;
        rb2d.velocity = (endVector - new Vector2(transform.position.x, transform.position.y)) * SHOOT_FORCE;
        StartCoroutine("SlowDown");
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

    private IEnumerator SlowDown()
    {
        yield return null;

        int counter = 0;
        while (counter < 10)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x / 1.1f, rb2d.velocity.y / 1.1f);
            counter++;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return null;
        rb2d.velocity = new Vector2(0, 0);
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
