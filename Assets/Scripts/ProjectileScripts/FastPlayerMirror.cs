using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastPlayerMirror : MonoBehaviour
{
    public GameObject explosionPrefab;

    private Rigidbody2D rb2d;

    private static string ENEMY_TAG = "Enemy";

    private const float SHOOT_FORCE = 5f;

    // Use this for initialization
    void Start()
    {
        this.rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 5f);
    }

    public void SetPathVector(Vector2 endVector)
    {
        this.rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine("SlowDown", endVector);
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

    private IEnumerator SlowDown(Vector2 endVector)
    {
        while (Mathf.Abs(endVector.magnitude - transform.position.magnitude) > 0.01f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            transform.position = Vector2.Lerp(transform.position, endVector, 1 / 10.0f);
        }
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
