using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public GameObject explosionPrefab;

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
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(2f);
        Explode();
    }

    private void Explode()
    {
        Instantiate(explosionPrefab, gameObject.transform.position,
                    gameObject.transform.rotation);
        Destroy(this.gameObject);
    }
}
