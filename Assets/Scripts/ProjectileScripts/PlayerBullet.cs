using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public GameObject explosionPrefab;

    private static string PLAYER_TAG = "Player";
    private static string ENEMY_TAG = "Enemy";

    // Use this for initialization
    void Start()
    {
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

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(1.5f);
        Explode();
    }

    private void Explode()
    {
        Instantiate(explosionPrefab, gameObject.transform.position,
                    gameObject.transform.rotation);
        Destroy(this.gameObject);
    }
}
