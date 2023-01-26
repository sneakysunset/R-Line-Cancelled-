using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class TestCross : MonoBehaviour
{
    public Vector3 pC, pL, pR;
    public Vector3 pL2;
    public float Length;
    public Vector3 resultPoint, resultPoint2;
    public float angle, angle2;
    public float gizmoPointSize;
    public float result2Lenght;

    void Update()
    {
        Vector3 pCL = (pL - pC).normalized;
        Vector3 pCR = (pR - pC).normalized;
        angle = Vector3.SignedAngle(pCL, pCR, Vector3.forward) / 2;  
        resultPoint =  ((Quaternion.AngleAxis(-Mathf.Abs(angle), Vector3.forward) * pCL) ).normalized * Length;


        Vector3 pCL2 = (pL2 - pL).normalized;
        Vector3 pCR2 = (pC - pL).normalized;
        angle2 = Vector3.SignedAngle(pCL2, pCR2, Vector3.forward) / 2;
        
        resultPoint2 = ((Quaternion.AngleAxis(-Mathf.Abs(angle2), Vector3.forward) * pCL2)).normalized * Length;
        result2Lenght = resultPoint2.magnitude;
    }

    private void OnDrawGizmos()
    {

       Gizmos.color = Color.white;
       Gizmos.DrawCube(pC, Vector3.one * gizmoPointSize);
       Gizmos.DrawCube(pL, Vector3.one * gizmoPointSize);
       Gizmos.DrawCube(pR, Vector3.one * gizmoPointSize);
       Gizmos.DrawCube(pL2, Vector3.one * gizmoPointSize);
       Gizmos.color = Color.yellow;
       Gizmos.DrawCube(resultPoint, Vector3.one * gizmoPointSize * 2);
       Gizmos.DrawCube(resultPoint2, Vector3.one * gizmoPointSize * 2);

       Debug.DrawLine(pC, pL, Color.blue);
       Debug.DrawLine(pL2, pL, Color.blue);
       Debug.DrawLine(pC, pR, Color.blue);
       Debug.DrawLine(pC, resultPoint, Color.red);
       Debug.DrawLine(pL, resultPoint2, Color.red);
       Debug.DrawLine(resultPoint, resultPoint2, Color.red);

    }
}
