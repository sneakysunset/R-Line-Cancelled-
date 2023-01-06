using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineCreator : MonoBehaviour
{
    #region Variables
    private float[] pointArray;
    public List<Vector2> pointList = new List<Vector2>();
    private Color col;
    private CharacterController2D charC;
    Transform lineFolder;
    [HideInInspector] public LineRenderer lineR;
    [HideInInspector] public EdgeCollider2D edgeC;
    [HideInInspector] public Transform lineT;

    private CharacterController2D.Team pType;
    Collider coll;
    [Header("Components")]
    [Space(5)]
    public GameObject linePrefab;
    Vector2 ogPos;
    int prevUpdatedIndex;
    public GameObject ballPrefab;



    [Space(10)]
    [Header("Line Variables")]
    [Space(5)]
    [Range(.001f, 1)] public float lineResolution = .5f;
    public float lineBeginningX = -13f;
    public float lineEndX = 13f;
    public float width = .3f;
    public float lineYOffSet = 0;

    [Space(10)]
    [Header("Refresh Variables")]
    [Space(5)]
    public float threshHoldToUpdatePoints;
    public float updateSoundFrequency;
    public AnimationCurve updateAnim;
    public float updateAnimSpeed;
    #endregion

    private void Start()
    {
        lineFolder = GameObject.FindGameObjectWithTag("LineFolder").transform;
        pointArray = Utils_Points.GeneratePointArray(pointArray, lineBeginningX, lineEndX, lineResolution);
        if (GetComponent<CharacterController2D>())
        {
            charC = GetComponent<CharacterController2D>();
            col = charC.col;
            pType = charC.playerType;
        }
        else pType = CharacterController2D.Team.Ball;
        ogPos = transform.position;
        var firstPoint = new Vector2(Utils_Points.closestPoint(pointArray, transform.position.x), transform.position.y);
        pointList.Add(firstPoint);
        InstantiateLine();
    }

    private void InstantiateLine()
    {
        lineT = Instantiate(linePrefab, lineFolder).transform;
        lineR = lineT.GetComponentInChildren<LineRenderer>();
        edgeC = lineT.GetComponentInChildren<EdgeCollider2D>();
        lineR.material.color = col;
        lineT.name = "Mesh " + pType.ToString() + " Off";
        edgeC.gameObject.layer = 6;
        if (pType != CharacterController2D.Team.Ball)
            charC.meshObj = lineT.gameObject;
    }

    public void LineUpdater()
    {
        var list = pointList;
        list = pointList.OrderBy(v => v.x).ToList();
        pointList = list;

        if (!UpdatePointList()) return;

        list = pointList.OrderBy(v => v.x).ToList();
        pointList = list;

        if (list.Count < 4 || edgeC.gameObject.layer == 10) return;

        lineR.positionCount = pointList.Count;
        Vector3[] vector3s = new Vector3[pointList.Count];
        for (int i = 0; i < vector3s.Length; i++)
        {
            vector3s[i] = pointList[i];
        }
        lineR.SetPositions(vector3s);
        StartCoroutine(afterPhysics());
    }

    public bool UpdatePointList()
    {
        Vector2 pPos = new Vector2(transform.position.x, transform.position.y);
        bool condition1 = pointList[0].x - pPos.x > lineResolution || pPos.x - pointList[pointList.Count - 1].x > lineResolution;
        bool condition2 = pointList[0].x - pPos.x < lineResolution && pPos.x - pointList[pointList.Count - 1].x < lineResolution;
        bool condition3 = transform.position.x != prevUpdatedIndex;
        if (condition1)
        {
            AddPoint();
            return true;
        }
        else if (condition2 && condition3)
        {
            UpdatePoint();
            return true;
        }
        else return false;
    }

    void AddPoint()
    {
        Vector2 pPos = new Vector2(transform.position.x, transform.position.y);
        prevUpdatedIndex = -1;
        float posX = 0;
        int numOfAdded = 0;
        var a = pPos.x - Mathf.FloorToInt(pPos.x);
        var b = a - (a % lineResolution);
        if (pointList[0].x - pPos.x > lineResolution)
        {
            posX = Mathf.FloorToInt(pPos.x) + b + lineResolution;

            for (float i = posX; i < pointList[0].x; i += lineResolution)
            {
                float posY = Mathf.Lerp(pPos.y, pointList[0].y, numOfAdded / ((pointList[0].x - pPos.x) / lineResolution));
                pointList.Add(new Vector2(i, posY));
                numOfAdded++;
            }

        }
        else
        {
            posX = Mathf.FloorToInt(pPos.x) + b;

            for (float i = posX; i > pointList[pointList.Count - 1].x; i -= lineResolution)
            {
                float posY = Mathf.Lerp(pPos.y, pointList[0].y, numOfAdded / ((pPos.x - pointList[pointList.Count - 1].x) / lineResolution));
                pointList.Add(new Vector2(i, posY));
                numOfAdded++;
            }
        }
    }

    void UpdatePoint()
    {
        Vector2 pPos = new Vector2(transform.position.x, transform.position.y);

        float posX = Mathf.FloorToInt(pPos.x);

        float curDistance = 100000;
        int closestIndex = 10000;
        for (int i = 0; i < pointList.Count; i++)
        {
            if (Mathf.Abs(pointList[i].x - pPos.x) < curDistance)
            {
                closestIndex = i;
                curDistance = Mathf.Abs(pointList[i].x - pPos.x);
            }
        }


        Vector2 newPos = new Vector2(pointList[closestIndex].x, pPos.y);
        pointList[closestIndex] = newPos;

        if (prevUpdatedIndex != -1)
        {
            if (closestIndex - prevUpdatedIndex < 0)
            {
                for (int i = closestIndex; i < prevUpdatedIndex; i++)
                {
                    float posY = Mathf.Lerp(pointList[closestIndex].y, pointList[prevUpdatedIndex].y, (Mathf.Abs(i) - Mathf.Abs(closestIndex)) / (Mathf.Abs(prevUpdatedIndex - closestIndex)));
                    pointList[i] = new Vector2(pointList[i].x, posY);
                }
            }
            else
            {
                for (int i = closestIndex; i > prevUpdatedIndex; i--)
                {
                    float posY = Mathf.Lerp(pointList[prevUpdatedIndex].y, pointList[closestIndex].y, (Mathf.Abs(i) - Mathf.Abs(prevUpdatedIndex)) / (Mathf.Abs(closestIndex - prevUpdatedIndex)));
                    pointList[i] = new Vector2(pointList[i].x, posY);
                }
            }
        }
        else
        {
            pointList[closestIndex] = newPos;
        }

        prevUpdatedIndex = closestIndex;
    }

    IEnumerator afterPhysics()
    {
        yield return new WaitForFixedUpdate();
        edgeC.SetPoints(pointList);
    }

    private void OnDestroy()
    {
        if (lineT)
            Destroy(lineT.gameObject);

        // Instantiate(ballPrefab, ogPos, Quaternion.identity);
    }

}
