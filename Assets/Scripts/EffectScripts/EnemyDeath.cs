using UnityEngine;
using System.Collections;

public class EnemyDeath : MonoBehaviour
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
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

}
