using System.Collections;
using UnityEngine;
using Enemy;

public class Crab : EnemyScript
{
    private float h = 0f;
    private float position = 0f;
    private float checkTime = 0.1f;
    private float time = 0f;

    private void Start()
    {
        base.Start(5);
        h = Random.Range(-1f, 1f) > 0 ? 1f : -1f;
        position = gameObject.transform.position.x;
        StartCoroutine("HaltMovement");
    }

    private void FixedUpdate()
    {
        time += Time.deltaTime;

        if (time > checkTime)
        {
            time = 0f;
            if (Mathf.Approximately(GetComponent<Transform>().position.x, position) && !Mathf.Approximately(h, 0))
            {
                h *= -1;
            }
            position = GetComponent<Transform>().position.x;
        }

        CheckFlip(h);
        Move(h);
    }

    void OnCollisionEnter2D(Collision2D collisionObject)
    {
        string objectTag = collisionObject.gameObject.tag;
        if (objectTag.Equals(Constants.PLAYER_PROJECTILE_TAG))
        {
            gameObject.SendMessage(
                "TakeDamage",
                1,
                SendMessageOptions.DontRequireReceiver
            );
            return;
        }
    }

    private IEnumerator HaltMovement()
    {
        yield return new WaitForSeconds(2f);
        h = 0;
        StartCoroutine("StartMovement");
    }

    private IEnumerator StartMovement()
    {
        time = 0;
        yield return new WaitForSeconds(2f);
        h = Random.Range(-1f, 1f) > 0 ? 1f : -1f;
        StartCoroutine("HaltMovement");
    }


}
