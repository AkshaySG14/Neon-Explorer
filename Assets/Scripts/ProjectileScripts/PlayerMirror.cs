using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectileConsts;

public abstract class PlayerMirror : MonoBehaviour
{

    public GameObject explosionPrefab;

    protected Rigidbody2D rb2d;

    protected static string ENEMY_TAG = "Enemy";

    protected float shootForce;
    protected float lifeSpan = 1f;

    protected bool spawning = true;

    // Use this for initialization
    public void Start()
    {
        this.GetComponent<Renderer>().enabled = false;
        this.rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
    }

    public abstract void SetPathVector(Vector2 pathVector);

    protected void Explode()
    {
        Instantiate(explosionPrefab, gameObject.transform.position,
                    gameObject.transform.rotation);
        Destroy(this.gameObject);
    }

    protected IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifeSpan);
        Explode();
    }
}
