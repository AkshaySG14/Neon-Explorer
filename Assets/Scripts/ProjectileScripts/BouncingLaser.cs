using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BouncingLaser : MonoBehaviour
{

    public int laserDistance;
    public string tileTag;
    public string mirrorTag;
    public int collisionsPerFrame;
    public int collisionMisses;
    public int maxNumberOfLasers;
    public int maxReflections;

    private int reflections;
    private int number = 0;
    private int vertexCounter = 1;
    private int missedCollisionFrames = 0;

    private float time = 0.75f;
    private bool justCollided = false;

    public GameObject laser;

    private BouncingLaser parentLaser = null;
    private BouncingLaser childLaser = null;
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

    public void SetParent(BouncingLaser bouncingLaser)
    {
        this.parentLaser = bouncingLaser;
        parentLaser.SendMessage("SetChildLaser", this);
    }

    public void SetReflections(int reflections)
    {
        this.mLineRenderer = GetComponent<LineRenderer>();
        this.reflections = reflections;
        switch (reflections)
        {
            case 0:
                mLineRenderer.startColor = Color.blue;
                mLineRenderer.endColor = Color.blue;
                break;
            case 1:
                mLineRenderer.startColor = Color.cyan;
                mLineRenderer.endColor = Color.cyan;
                break;
            case 2:
                mLineRenderer.startColor = Color.green;
                mLineRenderer.endColor = Color.green;
                break;
            case 3:
                mLineRenderer.startColor = Color.magenta;
                mLineRenderer.endColor = Color.magenta;
                break;
            case 4:
                mLineRenderer.startColor = Color.yellow;
                mLineRenderer.endColor = Color.yellow;
                break;
            default:
                break;
        }
    }

    public void SetNumber(int number)
    {
        this.number = number;
    }

    public void SetMissedCollisionFrames(int misses)
    {
        this.missedCollisionFrames = misses;
    }

    public void SetPathVector(Vector3 pathVector)
    {
        this.pathVector = pathVector.normalized;
        Vector2 transformVector = new Vector2(pathVector.x, pathVector.y);
        transformVector = transformVector.normalized;
        this.pathVector = new Vector3(transformVector.x, transformVector.y, 0);
        StartCoroutine("Fire");
    }

    public void SetChildLaser(BouncingLaser laser)
    {
        this.childLaser = laser;
    }

    public void SetTime(float time)
    {
        this.time = time;
    }

    public void SetCollided(bool justCollided)
    {
        this.justCollided = justCollided;
    }

    IEnumerator Destruct()
    {
        if (parentLaser)
        {
            StartCoroutine("ParentDestruct");
        }
        else
        {
            StartCoroutine("ChildDestruct");
        }
        yield return null;
    }

    IEnumerator ChildDestruct()
    {
        yield return null;
        if (childLaser)
        {
            childLaser.SendMessage("ChildDestruct");
        }
        float alpha = 0.5f;
        while (alpha > 0)
        {
            alpha -= 0.05f;
            mLineRenderer.material.SetColor("_TintColor", new Color(1f, 1f, 1f, alpha));
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Destroy(this.gameObject);
    }

    IEnumerator ParentDestruct()
    {
        parentLaser.SendMessage("Destruct", this);
        yield return null;
    }

    IEnumerator Fire()
    {
        const float DIST_INTERVAL = 0.25f;

        Vector3 pos = transform.position;
        mLineRenderer.SetPosition(0, pos);
        mLineRenderer.positionCount = 1;

        LayerMask blockLayer = LayerMask.GetMask("Blocking");
        LayerMask mirrorLayer = LayerMask.GetMask("Mirror");

        for (int i = 0; i < collisionsPerFrame; i++)
        {
            RaycastHit2D tileHit2D = Physics2D.Raycast(pos, pathVector, DIST_INTERVAL / collisionsPerFrame, blockLayer);
            if (tileHit2D && missedCollisionFrames <= collisionMisses)
            {
                if (tileHit2D.collider.tag == tileTag)
                {
                    Reflect(tileHit2D);
                    yield break;
                }
                else
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                }
            }
            else
            {
                RaycastHit2D mirrorHit2D = Physics2D.Raycast(pos, pathVector, DIST_INTERVAL / collisionsPerFrame, mirrorLayer);
                if (mirrorHit2D && missedCollisionFrames <= collisionMisses)
                {
                    if (mirrorHit2D.collider.tag == mirrorTag)
                    {
                        Reflect(mirrorHit2D);
                        yield break;
                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(0.5f);
                    }
                }
                else
                {
                    mLineRenderer.positionCount++;
                    pos += pathVector * DIST_INTERVAL / collisionsPerFrame;
                    mLineRenderer.SetPosition(vertexCounter++, pos);
                }
            }
            justCollided = false;
            missedCollisionFrames--;
        }
        time -= Time.deltaTime;
        if (time <= 0)
        {
            StartCoroutine("Destruct", null);
            yield break;
        }
        yield return new WaitForSecondsRealtime(0.01f);
        GameObject laserProj = Instantiate(laser, pos, transform.rotation);
        laserProj.SendMessage("SetParent", this);
        laserProj.SendMessage("SetReflections", reflections);
        laserProj.SendMessage("SetNumber", number + 1);
        laserProj.SendMessage("SetTime", time);
        laserProj.SendMessage("SetCollided", false);
        laserProj.SendMessage("SetPathVector", pathVector);
    }

    private void Reflect(RaycastHit2D hit2D)
    {
        if (justCollided)
        {
            StartCoroutine("Destruct", null);
            return;
        }
        justCollided = true;
        mLineRenderer.positionCount++;
        mLineRenderer.SetPosition(vertexCounter++, hit2D.point);
        GameObject laserProj = Instantiate(laser, hit2D.point, transform.rotation);
        laserProj.SendMessage("SetParent", this);
        laserProj.SendMessage("SetReflections", reflections + 1);
        laserProj.SendMessage("SetMissedCollisionFrames", 1);
        laserProj.SendMessage("SetTime", time);
        laserProj.SendMessage("SetCollided", true);
        laserProj.SendMessage("SetPathVector", Vector3.Reflect(pathVector, hit2D.normal));
    }

    private void End(RaycastHit2D raycastHit2D)
    {
        mLineRenderer.positionCount++;
        mLineRenderer.SetPosition(vertexCounter, raycastHit2D.point);
        Destruct();
    }
}