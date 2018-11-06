using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastPlayerMirror : MonoBehaviour
{
    public GameObject explosionPrefab;

    private Rigidbody2D rb2d;

    private static string ENEMY_TAG = "Enemy";

    private const float SHOOT_FORCE = 5f;

    private bool spawning = true;

    private Vector2 endVector;

    // Use this for initialization
    void Start()
    {
        this.GetComponent<Renderer>().enabled = false;
        this.rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 5f);
    }

    public void SetPathVector(Vector2 endVector)
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
            rb2d.velocity = new Vector2(endVector.x - transform.position.x, endVector.y - transform.position.y);
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
