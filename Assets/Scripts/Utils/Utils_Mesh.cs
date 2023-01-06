using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Utils_Mesh 
{
    public static void UpdateMeshVertices(List<Vector2> pointList, float lineWidth, Mesh m)
    {
        int listLength = 2 * pointList.Count - 1;
        Vector3[] vertices = new Vector3[listLength];


        for (int i = 0; i < vertices.Length; i++)
        {
            if (i == vertices.Length - 1 || i == 1)
            {
                vertices[i] = pointList[(i - 1) / 2] + new Vector2(0, lineWidth);
            }
            else if (i % 2 == 0)
            {
                vertices[i] = pointList[i / 2];
            }
            else
            {
                int pI = (i - 1) / 2;
                //vertices[i] = pointList[pI] + new Vector2(0, lineWidth);
                vertices[i] = Utils_Points.GetParallelePoint(pointList[pI], pointList[pI - 1], pointList[pI + 1], lineWidth);

            }
        }
        //vertices = vertices.OrderBy(v => v.x).ToArray();

        m.vertices = vertices;
    }

    public static void UpdateMeshTriangles(int pointListLength, Mesh m)
    {
        int[] triangles = new int[6 * pointListLength - 9];
        int j = 0;
        int k = 0;
        int f = 0;
        for (int i = 0; i < triangles.Length; i++)
        {
            if (j == 6)
            {
                j = 0;
                f++;
            }

            if (j <= 3) k = 0;
            else if (j == 4) k = -2;
            else if (j == 5) k = -4;
            j++;

            triangles[i] = i + k - 4 * f;
        }
        m.triangles = triangles;
    }

    public static IEnumerator MeshAbsorption(float timer, float num, List<Vector2> pointList, CharacterController2D charC, Action endOfCoroutine = null)
    {
        yield return new WaitForSeconds(timer);
        int i = 0;
        while (i < num)
        {
            i++;
            pointList.RemoveAt(0);
            charC.transform.localScale += Vector3.one * charC.movementScaler * Time.deltaTime;
        }
        endOfCoroutine.Invoke() ;
    }
}
