using System.Collections;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private static string ENEMY_TAG = "Enemy";

    void OnCollisionEnter2D(Collision2D collisionObject)
    {
        string objectTag = collisionObject.gameObject.tag;
        if (objectTag.Equals(ENEMY_TAG))
        {
            gameObject.SendMessage(
                "TakeDamage",
                1,
                SendMessageOptions.DontRequireReceiver
            );
            return;
        }
    }

    private IEnumerator ReEnableCollision(Collider2D objCollider)
    {
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(
            objCollider,
            gameObject.GetComponent<Collider2D>(),
            false
        );
    }
}
