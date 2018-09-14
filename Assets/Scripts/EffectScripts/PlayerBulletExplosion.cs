using UnityEngine;
using System.Collections;

public class PlayerBulletExplosion : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        StartCoroutine("SelfDestruct");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(0.33f);
        Destroy(gameObject);
    }

}
