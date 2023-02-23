using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MovingPlat : MonoBehaviour
{
    [SerializeField, HideInInspector] public Vector3[] keysPos;
    [SerializeField] public int numberOfKeys;
    public GameObject KeyPoint;
    [SerializeField, HideInInspector] public Vector3 prevPos;
    LineRenderer lineR;
    int index;
    int prevIndex;
    float moveCursor;

    public int firstIndex;
    public bool moving;
    public float movingSpeed;
    public bool ascending = true;
    public bool loop;
    private void Start()
    {
        lineR = GetComponent<LineRenderer>();

        prevIndex = firstIndex;
        transform.position = keysPos[prevIndex];

        if (!loop)
        {
            lineR.positionCount = keysPos.Length;
            for (int i = 0; i < keysPos.Length; i++)
            {
                Instantiate(KeyPoint, keysPos[i], Quaternion.identity);
                lineR.SetPosition(i, keysPos[i]);
            }
        }
        else
        {
            lineR.positionCount = keysPos.Length + 1;
            for (int i = 0; i < keysPos.Length; i++)
            {
                Instantiate(KeyPoint, keysPos[i], Quaternion.identity);
                lineR.SetPosition(i, keysPos[i]);
            }
            lineR.SetPosition(lineR.positionCount - 1, keysPos[0]);
        }
    }

    private void FixedUpdate()
    {
        ChangeIndex();
        if (moving) MoveTile();
    }

    void MoveTile()
    {
        moveCursor += Time.deltaTime * movingSpeed;
        transform.position = Vector3.MoveTowards(keysPos[prevIndex], keysPos[index], moveCursor);
    }

    void ChangeIndex()
    {
        if (!loop)
        {
            if (transform.position == keysPos[index])
            {
                moveCursor = 0;
                if (ascending && index == numberOfKeys - 1) ascending = false;
                else if (!ascending && index == 0) ascending = true;
                prevIndex = index;

                if (ascending)
                    index++;
                else index--;
            }
        }
        else
        {
            if (transform.position == keysPos[index])
            {
                moveCursor = 0;
                prevIndex = index;
                if (ascending && index == keysPos.Length - 1) index = 0;
                else if (!ascending && index == 0) index = keysPos.Length - 1;
                else if (ascending) index++;
                else if (!ascending) index--;
            }
        }
    }

    public void StopMoving(bool move)
    {
        if (move) moving = true;
        else moving = false;
    }
}


#if UNITY_EDITOR
#if !Play
[CustomEditor(typeof(MovingPlat))]
public class MovingTile_Editor : Editor
{
    MovingPlat moveT;

    private void OnSceneGUI()
    {
        if(!Application.isPlaying)
            Draw();
    }

    void Draw()
    {
        Vector3[] vecs = moveT.keysPos;
        moveT.keysPos = new Vector3[moveT.numberOfKeys];
        for (int i = 0; i < moveT.keysPos.Length; i++)
        {
            if (vecs.Length > i) moveT.keysPos[i] = vecs[i];
            else if (i > 0) moveT.keysPos[i].x = moveT.keysPos[i - 1].x + 1;
        }

        MoveHandlesAlongObject();

        for (int i = 0; i < moveT.keysPos.Length - 1; i++)
        {
            Handles.color = Color.black;
            Handles.DrawLine(moveT.keysPos[i], moveT.keysPos[i + 1]);

            Handles.color = Color.red;
            Vector3 newPosA = Handles.FreeMoveHandle(moveT.keysPos[i], Quaternion.identity, .5f, Vector3.zero, Handles.CylinderHandleCap);
            if (moveT.keysPos[i] != newPosA)
            {
                Undo.RecordObject(moveT, "MovePoint");
                MovePoint(newPosA, i);
            }
        }
        if (moveT.loop)
        {
            Handles.color = Color.black;
            Handles.DrawLine(moveT.keysPos[moveT.keysPos.Length - 1], moveT.keysPos[0]);
        }

        Handles.color = Color.red;
        Vector3 newPos = Handles.FreeMoveHandle(moveT.keysPos[moveT.keysPos.Length - 1], Quaternion.identity, .5f, Vector3.zero, Handles.CylinderHandleCap);
        if (moveT.keysPos[moveT.keysPos.Length - 1] != newPos)
        {
            Undo.RecordObject(moveT, "MovePoint");
            MovePoint(newPos, moveT.keysPos.Length - 1);
        }
    }

    private void OnEnable()
    {
        moveT = (MovingPlat)target;
    }

    void MovePoint(Vector3 pos, int i)
    {
        moveT.keysPos[i] = new Vector3(pos.x, pos.y, 0);
        EditorUtility.SetDirty(moveT);
    }

    void MoveHandlesAlongObject()
    {
        if(moveT.prevPos != moveT.transform.position)
        {
            Vector3 movement =  moveT.transform.position - moveT.prevPos;
            for (int i = 0; i < moveT.keysPos.Length; i++) moveT.keysPos[i] += movement;
            moveT.prevPos = moveT.transform.position;
        }
    }
}
#endif
#endif