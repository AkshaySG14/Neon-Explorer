using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BouncingLaser : MonoBehaviour
{

    public int laserDistance;
    public string tileTag;
    public string mirrorTag;
    public int maxBounce;
    public int collisionsPerFrame;
    public int collisionMisses;

    private bool active = true;
    private int reflections;
    private int vertexCounter = 1;

    public GameObject laser;

    private BouncingLaser parentLaser = null;
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
    }

    public void SetReflections(int reflections)
    {
        this.mLineRenderer = GetComponent<LineRenderer>();
        this.reflections = reflections;
        switch (reflections)
        {
            case 0:
                mLineRenderer.startColor = Color.blue;
                mLineRenderer.endColor = Color.cyan;
                break;
            case 1:
                mLineRenderer.startColor = Color.cyan;
                mLineRenderer.endColor = Color.green;
                break;
            case 2:
                mLineRenderer.startColor = Color.green;
                mLineRenderer.endColor = Color.magenta;
                break;
            case 3:
                mLineRenderer.startColor = Color.magenta;
                mLineRenderer.endColor = Color.yellow;
                break;
            case 4:
                mLineRenderer.startColor = Color.yellow;
                mLineRenderer.endColor = Color.red;
                break;
            default:
                break;
        }
    }

    public void SetPathVector(Vector3 pathVector)
    {
        this.pathVector = pathVector.normalized;
        Vector2 transformVector = new Vector2(pathVector.x, pathVector.y);
        transformVector = transformVector.normalized;
        this.pathVector = new Vector3(transformVector.x, transformVector.y, 0);
        StartCoroutine("Fire");
    }

    IEnumerator Destruct()
    {
        active = false;
        if (parentLaser)
        {
            StartCoroutine(parentLaser.Destruct());
        }
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(this.gameObject);
    }

    IEnumerator Fire()
    {
        int missedCollisionFrames = 0;
        const float DIST_INTERVAL = 0.2f;
        const float TIME_MAX = 0.5f;
        float time = 0;

        Vector3 pos = transform.position;
        mLineRenderer.SetPosition(0, pos);
        mLineRenderer.positionCount = 1;

        LayerMask blockLayer = LayerMask.GetMask("Blocking");
        LayerMask projectileLayer = LayerMask.GetMask("Projectile");

        while (active)
        {
            for (int i = 0; i < collisionsPerFrame; i++)
            {
                RaycastHit2D tileHit2D = Physics2D.Raycast(pos, pathVector, DIST_INTERVAL / collisionsPerFrame, blockLayer);
                if (tileHit2D && missedCollisionFrames > collisionMisses)
                {
                    if (tileHit2D.collider.tag == tileTag)
                    {
                        Reflect(tileHit2D);
                        break;
                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(0.5f);
                    }
                }
                else
                {
                    RaycastHit2D mirrorHit2D = Physics2D.Raycast(pos, pathVector, DIST_INTERVAL / collisionsPerFrame, projectileLayer);
                    if (mirrorHit2D && missedCollisionFrames > collisionMisses)
                    {
                        if (mirrorHit2D.collider.tag == mirrorTag)
                        {
                            Reflect(mirrorHit2D);
                            break;
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

                if (time > TIME_MAX)
                {
                    StartCoroutine("Destruct");
                }

                time += 0.01f;
                missedCollisionFrames++;
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    private void Reflect(RaycastHit2D hit2D)
    {
        if (reflections + 1 > maxBounce)
        {
            StartCoroutine("Destruct");
        }
        else
        {
            active = false;
            mLineRenderer.positionCount++;
            mLineRenderer.SetPosition(vertexCounter++, hit2D.point);
            GameObject laserProj = Instantiate(laser, hit2D.point, transform.rotation);
            laserProj.SendMessage("SetParent", this);
            laserProj.SendMessage("SetReflections", reflections + 1);
            laserProj.SendMessage("SetPathVector", Vector3.Reflect(pathVector, hit2D.normal));
        }
    }

    private void End(RaycastHit2D raycastHit2D)
    {
        mLineRenderer.positionCount++;
        mLineRenderer.SetPosition(vertexCounter, raycastHit2D.point);
        Destruct();
    }
}