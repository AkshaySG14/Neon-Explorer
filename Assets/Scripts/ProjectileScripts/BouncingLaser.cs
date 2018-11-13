using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BouncingLaser : MonoBehaviour
{

    public int laserDistance;
    public string bounceTag;
    public string playerTag;
    public int maxBounce;

    private LineRenderer mLineRenderer;
    private Vector3 pathVector;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetPathVector(Vector3 pathVector)
    {
        this.pathVector = pathVector.normalized;
        Vector2 transformVector = new Vector2(pathVector.x, pathVector.y);
        transformVector = transformVector.normalized;
        this.pathVector = new Vector3(transformVector.x, transformVector.y, 0);
        mLineRenderer = GetComponent<LineRenderer>();
        StartCoroutine("Fire");
    }


    IEnumerator Fire()
    {
        float time = 0;
        const float distInterval = 0.25f;

        float TIME_MAX = 0.25f;

        int laserReflected = 0; //How many times it got reflected
        int vertexCounter = 1; //How many line segments are there
        bool loopActive = true; //Is the reflecting loop active?

        Vector3 pos = transform.position;
        mLineRenderer.SetPosition(0, pos);
        mLineRenderer.positionCount = 1;

        LayerMask layerMask = LayerMask.GetMask("Blocking");

        while (loopActive)
        {
            RaycastHit2D hit2D = Physics2D.Raycast(pos, pathVector, distInterval, layerMask);
            if (hit2D)
            {
                if (hit2D.collider.tag == bounceTag)
                {
                    laserReflected++;
                    mLineRenderer.positionCount++;
                    mLineRenderer.SetPosition(vertexCounter++, hit2D.point);
                    pathVector = Vector3.Reflect(pathVector, hit2D.normal);
                }
                else
                {
                    mLineRenderer.positionCount++;
                    mLineRenderer.SetPosition(vertexCounter, hit2D.point);
                    loopActive = false;
                    yield return new WaitForSecondsRealtime(0.5f);
                    Destroy(this.gameObject);
                }
            }
            else
            {
                mLineRenderer.positionCount++;
                pos += pathVector * distInterval;
                mLineRenderer.SetPosition(vertexCounter++, pos);
            }

            if (time > TIME_MAX || laserReflected > maxBounce)
            {
                loopActive = false;
                yield return new WaitForSecondsRealtime(0.5f);
                Destroy(this.gameObject);
            }

            time += 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    private void Explode()
    {
        Destroy(this.gameObject);
    }
}