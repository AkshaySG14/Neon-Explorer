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
        float dist = 0.1f;
        float distInterval = 0.25f;

        float TIME_MAX = 0.25f;

        int laserReflected = 1; //How many times it got reflected
        int vertexCounter = 1; //How many line segments are there
        bool loopActive = true; //Is the reflecting loop active?

        mLineRenderer.SetPosition(0, transform.position);
        mLineRenderer.positionCount = 1;

        while (loopActive)
        {
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, pathVector, distInterval * (1.1f));
            if (hit2D && hit2D.transform.gameObject.tag != playerTag)
            {
                mLineRenderer.positionCount++;
                mLineRenderer.SetPosition(vertexCounter, hit2D.point);
                loopActive = false;
            }
            else
            {
                mLineRenderer.positionCount++;
                mLineRenderer.SetPosition(vertexCounter++, transform.position + pathVector * dist);
                dist += distInterval;
            }

            if (time > TIME_MAX)
            {
                loopActive = false;
            }

            time += 0.01f;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        //mLineRenderer.SetPosition(0, transform.position);
        //mLineRenderer.SetPosition(1, transform.position);
        //while (loopActive)
        //{

        //    if (Physics.Raycast(lastLaserPosition, laserDirection, out hit, laserDistance) && hit.transform.gameObject.tag == bounceTag && hit.transform.gameObject.tag != playerTag)
        //    {

        //        Debug.Log("Bounce");
        //        laserReflected++;
        //        vertexCounter += 3;
        //        mLineRenderer.positionCount = vertexCounter;
        //        mLineRenderer.SetPosition(vertexCounter - 3, Vector3.MoveTowards(hit.point, lastLaserPosition, 0.01f));
        //        mLineRenderer.SetPosition(vertexCounter - 2, hit.point);
        //        mLineRenderer.SetPosition(vertexCounter - 1, hit.point);
        //        mLineRenderer.startWidth = .1f;
        //        mLineRenderer.endWidth = .1f;
        //        lastLaserPosition = hit.point;
        //        laserDirection = Vector3.Reflect(laserDirection, hit.normal);
        //    }
        //    else
        //    {
        //        Debug.Log("No Bounce");
        //        laserReflected++;
        //        vertexCounter++;
        //        mLineRenderer.positionCount = vertexCounter;
        //        Vector3 lastPos = lastLaserPosition + (laserDirection.normalized * laserDistance);
        //        Debug.Log("InitialPos " + lastLaserPosition + " Last Pos" + lastPos);
        //        mLineRenderer.SetPosition(vertexCounter - 1, lastLaserPosition + (laserDirection.normalized * laserDistance));
        //        loopActive = false;
        //    }
        //    if (laserReflected > maxBounce)
        //    {
        //        loopActive = false;
        //    }
        //}

        //if (Input.GetKey("space") && timer < 2)
        //{
        //    yield return new WaitForEndOfFrame();
        //    timer += Time.deltaTime;
        //    StartCoroutine("FireMahLazer");
        //}
        //else
        //{
        //    yield return null;
        //    mLineRenderer.enabled = false;
        //}
    }

    private void Explode()
    {
        Destroy(this.gameObject);
    }
}