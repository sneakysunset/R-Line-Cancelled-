using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class ConstantMeshGeneration : MonoBehaviour
{
    #region Variables
    /*[SerializeField]*/ private float[] pointArray;
    public List<Vector2> pointList = new List<Vector2>();
    private Color col;
    private CharacterController2D charC;
    Transform meshFolder;
    [HideInInspector] public MeshFilter meshF;
    private MeshCollider meshC;
    private MeshRenderer meshR;
    private CharacterController2D.Team pType;
    Collider coll;
    [Header("Components")]
    [Space(5)]
    public GameObject meshPrefab;



    [Space(10)]
    [Header("Line Variables")]
    [Space (5)]
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
        meshFolder = GameObject.FindGameObjectWithTag("MeshFolder").transform;
        pointArray = Utils_Points.GeneratePointArray(pointArray, lineBeginningX, lineEndX, lineResolution);
        if (GetComponent<CharacterController2D>())
        {
            charC = GetComponent<CharacterController2D>();
            col = charC.col;
            pType = charC.playerType;
        }
        else pType = CharacterController2D.Team.Ball;

        var firstPoint = new Vector2(Utils_Points.closestPoint(pointArray, transform.position.x), transform.position.y);
        pointList.Add(firstPoint);
        InstantiateMesh();
    }

    public void MeshCreator()
    {
        var list = pointList;
        list = pointList.OrderBy(v => v.x).ToList();

        if (!UpdatePointList()) return;

        if (list.Count < 4 || meshF.gameObject.layer == 10) return;

        
        Mesh m = new Mesh();
        m.name = "trailMesh";

        Utils_Mesh.UpdateMeshVertices(list, width, m, false);
        Utils_Mesh.UpdateMeshTriangles(list.Count, m);
        m.MarkDynamic();
        m.Optimize();
        m.OptimizeReorderVertexBuffer();
        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();
        meshF.mesh = m;
        meshC.sharedMesh = null;
        meshC.sharedMesh = m;
    }

    public bool UpdatePointList()
    {
        int closestVertexIndex = Utils_Points.ClosestPointInList(pointList, transform.position.x, pointArray);
        float closestVertexX = pointList[closestVertexIndex].x;

        BandAidRemovePoints();

        closestVertexIndex = Utils_Points.ClosestPointInList(pointList, transform.position.x, pointArray);
        closestVertexX = pointList[closestVertexIndex].x;

        bool condition2 = false;
        bool condition1 = Mathf.Abs(transform.position.x - closestVertexX) > lineResolution;
        if (pointList.Count > 4)
        {
            condition2 = Mathf.Abs(transform.position.y - pointList[closestVertexIndex].y) > threshHoldToUpdatePoints;
        }


        if (condition1)
        {
            int numOfPointAdded = Utils_Points.AddPoints(pointArray, pointList, closestVertexX, transform.position - Vector3.up * lineYOffSet, lineResolution, lineYOffSet);
            if(pType != CharacterController2D.Team.Ball)
                charC.transform.localScale -= Vector3.one * charC.movementScaler / 100 * numOfPointAdded;
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCorde/TirageCorde");
            return true;
        }
        else if (!condition1 && condition2)
        {
            var startPos = pointList[closestVertexIndex];
            var endPos = (Vector2)transform.position - (Vector2.up * lineYOffSet);
            StartCoroutine(Utils_Anim.AnimationLerp(startPos, endPos, endPos, updateAnim, updateAnimSpeed, returnValue => { pointList[closestVertexIndex] = returnValue; }));
            return true;
        }
        else
        {
            return false;
        }
    }


    private void InstantiateMesh()
    {
        GameObject temp = Instantiate(meshPrefab, meshFolder);
        meshF = temp.GetComponent<MeshFilter>();
        meshC = temp.GetComponent<MeshCollider>();
        meshR = temp.GetComponent<MeshRenderer>();
        meshR.material.color = col;
        temp.name = "Mesh " + pType.ToString() + " Off";
        if(pType != CharacterController2D.Team.Ball)
            charC.meshObj = temp;
    }

    //Enleve les doublons, il faut trouver une alternative
    void BandAidRemovePoints()
    {
        for (int i = 1; i < pointList.Count; i++)
        {
            if (Mathf.Abs(pointList[i].x - pointList[i - 1].x) < lineResolution * .9f)
            {
                pointList.RemoveAt(i);
                if(pType != CharacterController2D.Team.Ball)
                    charC.transform.localScale += Vector3.one * charC.movementScaler / 100;
                return;
            }
        }
    }
}
