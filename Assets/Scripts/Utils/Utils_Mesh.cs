using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Utils_Mesh 
{
    public static Vector2[] UpdateMeshVertices(List<Vector2> pointList, float lineWidth, Mesh m , bool surface, Vector2[] uvList)
    {
        int listLength = 2 * pointList.Count - 1;
        Vector3[] vertices = new Vector3[listLength];
        Vector2[] uvs = new Vector2[listLength];
        float distance = 0;
        //Vector2 vecUv = Vector2.zero;
        #region surface
        /*        if (surface)
                {

                    Vector2[] tempArray1 = new Vector2[(listLength - 1) / 2 + 1];
                    Vector2[] tempArray2 = new Vector2[(listLength - 1) / 2];
                    int n = 0;
                    int k = 0;
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        if (i == vertices.Length - 1 || i == 1)
                        {
                            Debug.Log(tempArray1.Length + " " + vertices.Length + " " + n);
                            vertices[i] = Utils_Points.GetSurfacePoint(pointList[i / 2]);
                            tempArray1[n] = Utils_Points.GetSurfacePoint(pointList[i / 2]);
                            n++;
                        }
                        else if (i % 2 == 0)
                        {
                            vertices[i] = Utils_Points.GetSurfacePoint(pointList[i / 2]);
                            tempArray1[n] = Utils_Points.GetSurfacePoint(pointList[i / 2]);
                            n++;

                        }
                        else
                        {
                            int pI = (i - 1) / 2;
                            vertices[i] = pointList[pI];
                            tempArray2[k] = pointList[pI];
                            k++;
                        }
                    }
                    Debug.Log(vertices[vertices.Length - 1]);
                    Debug.Log("t1 =  " + tempArray1.Length);
                    Debug.Log("t2 =  " + tempArray2.Length);


                    for (int i = 0; i < (listLength - 1) / 2 - 1; i++)
                    {
                        uvs[i] = tempArray1[i];
                    }

                    for (int i = (listLength - 1) / 2; i < listLength - 1; i++)
                    {
                        uvs[i] = tempArray2[i - ((listLength - 1) / 2)];
                    }
                }
                else
                {*/
        #endregion
        for (int i = 0; i < vertices.Length; i++)
        {
            if(i%2 == 0 && i != 0) distance += Vector2.Distance(pointList[i / 2], pointList[i / 2 - 1]) + lineWidth / Vector2.Distance(pointList[i / 2], pointList[i / 2 - 1]);
            if (i == vertices.Length - 1 || i == 1)
            {
                vertices[i] = pointList[(i - 1) / 2] + new Vector2(0, lineWidth/2);
                //if(i == vertices.Length - 1) uvs[i] = uvList[uvList.Length - 1];
                //else uvs[i] = uvList[1];

               // Debug.Log(uvs[i]);

            }
            else if(i == 0)
            {
                vertices[i] = pointList[(i - 1) / 2] - new Vector2(0, lineWidth / 2);
                //uvs[i] = uvList[0];
                //Debug.Log(uvs[i]);

            }
            else if (i % 2 == 0 && i != 0)
            {
                //vertices[i] = pointList[i / 2];
                vertices[i] = Utils_Points.GetParallelePoint(pointList[i / 2], pointList[(i / 2) - 1], pointList[(i / 2) + 1], lineWidth / 2, false);

                //vecUv = uvList[UnityEngine.Random.Range(2, uvList.Length - 2)];

                //uvs[i] = vecUv;
                //Debug.Log(uvs[i]);

            }
            else
            {
                int pI = (i - 1) / 2;
                vertices[i] = Utils_Points.GetParallelePoint(pointList[pI], pointList[pI - 1], pointList[pI + 1], lineWidth/2, true);
                //uvs[i] = vecUv + Vector2.up * (1 / 3);
                //Debug.Log(uvs[i]);
            }

           // if (i == vertices.Length - 2) uvs[i] = uvList[uvList.Length - 2];
        }
        
        m.vertices = vertices;
        m.uv = uvs;
        return uvs;
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
