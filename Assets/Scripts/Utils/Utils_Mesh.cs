using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Utils_Mesh 
{
    public static Vector2[] UpdateMeshVertices(List<Vector2> pointList, float lineWidth, Mesh m, bool surface, Vector2[] uvList, int textC, bool debugger)
    {
        int listLength = 4 * pointList.Count - 4;
        //Vector3[] vertices = new Vector3[listLength];
        //Vector2[] uvs = new Vector2[listLength];
        Vector2 uvCord = Vector2.zero;
        
        #region square UVs [Current]
        /*vertices[0] = pointList[0] - new Vector2(0, lineWidth / 2);
        vertices[1] = pointList[0] + new Vector2(0, lineWidth / 2);
        vertices[2] = Utils_Points.GetParallelePoint(pointList[1], pointList[0], pointList[2], lineWidth / 2, false);
        vertices[3] = Utils_Points.GetParallelePoint(pointList[1], pointList[0], pointList[2], lineWidth / 2, true);
        vertices[listLength - 1] = pointList[pointList.Count - 1] + new Vector2(0, lineWidth / 2);
        vertices[listLength - 2] = pointList[pointList.Count - 1] - new Vector2(0, lineWidth / 2);
        vertices[listLength - 3] = Utils_Points.GetParallelePoint(pointList[pointList.Count - 2], pointList[pointList.Count - 3], pointList[pointList.Count - 1], lineWidth / 2, true);
        vertices[listLength - 4] = Utils_Points.GetParallelePoint(pointList[pointList.Count - 2], pointList[pointList.Count - 3], pointList[pointList.Count - 2], lineWidth / 2, false);
        uvs[0] = Vector2.zero;
        uvs[1] = Vector2.up / 3;
        uvs[2] = Vector2.right / 3;
        uvs[3] = Vector2.one / 3;
        uvs[listLength - 1] = Vector2.one;
        uvs[listLength - 2] = Vector2.right + Vector2.up * 2 / 3;
        uvs[listLength - 3] = Vector2.right * 2 / 3 + Vector2.up;
        uvs[listLength - 4] = Vector2.one * 2 / 3;
        for (int i = 1; i < vertices.Length / 4 -1; i++)
        {
            vertices[i * 4] = Utils_Points.GetParallelePoint(pointList[i], pointList[i - 1], pointList[i + 1], lineWidth / 2, false);
            vertices[i * 4 + 1] = Utils_Points.GetParallelePoint(pointList[i], pointList[i - 1], pointList[i + 1], lineWidth / 2, true);
            vertices[i * 4 + 2] = Utils_Points.GetParallelePoint(pointList[i + 1], pointList[i], pointList[i + 2], lineWidth / 2, false);
            vertices[i * 4 + 3] = Utils_Points.GetParallelePoint(pointList[i + 1], pointList[i], pointList[i + 2], lineWidth / 2, true);

            uvCord = uvList[(i - 1) % (uvList.Length - 1)];
            uvs[i * 4] = uvCord;
            uvs[i * 4 + 1] = uvCord + Vector2.up / 3;
            uvs[i * 4 + 2] = uvCord + Vector2.right / 3;
            uvs[i * 4 + 3] = uvCord + Vector2.up / 3 + Vector2.right / 3;
        }*/
        #endregion

        #region distance UVs [Trying]
        List<Vector3> verticess = new List<Vector3>();
        List<Vector2> uvss = new List<Vector2>();
        int n = 0;
        float distance = 0;
        verticess.Add(pointList[0] - new Vector2(0, lineWidth / 2));
        verticess.Add(pointList[0] + new Vector2(0, lineWidth / 2));
        uvss.Add(uvList[0]);
        uvss.Add(uvList[0] + Vector2.up / 3);
        for (int i = 1; i < pointList.Count - 1; i++)
        {
            if (i > 1 && distance + Vector2.Distance(pointList[i], pointList[i - 1]) >= lineWidth)
            {
                //Vector2 tempPoint = pointList[i - 1] + (pointList[i - 1] - pointList[i]) * (lineWidth - distance);
                Vector2 tempPoint = Vector2.Lerp(pointList[i - 1], pointList[i], (lineWidth - distance) / Vector2.Distance(pointList[i], pointList[i - 1]));
                verticess.Add(Utils_Points.GetParallelePoint(tempPoint, pointList[i - 1], pointList[i], lineWidth / 2, false));
                verticess.Add(Utils_Points.GetParallelePoint(tempPoint, pointList[i - 1], pointList[i], lineWidth / 2, true));
                verticess.Add(Utils_Points.GetParallelePoint(tempPoint, pointList[i - 1], pointList[i], lineWidth / 2, false));
                verticess.Add(Utils_Points.GetParallelePoint(tempPoint, pointList[i - 1], pointList[i], lineWidth / 2, true));

                uvss.Add(uvCord + Vector2.right / 3);
                uvss.Add(uvCord + Vector2.one / 3);
                n++;
                int b = n % (uvList.Length - 2);
                if (b == 0) b++;
                uvCord = uvList[b];
                float dist = 0;
                for (int f = i; f < pointList.Count - 1; f++)
                {
                    dist += Vector2.Distance(pointList[f + 1], pointList[f]);
                }
                if (dist <= lineWidth) uvCord = uvList[uvList.Length - 1];

                uvss.Add(uvCord);
                uvss.Add(uvCord + Vector2.up / 3);
                distance = Vector2.Distance(pointList[i], tempPoint);
            }
            else distance += Vector2.Distance(pointList[i], pointList[i - 1]);

            verticess.Add(Utils_Points.GetParallelePoint(pointList[i], pointList[i - 1], pointList[i + 1], lineWidth / 2, false));
            verticess.Add(Utils_Points.GetParallelePoint(pointList[i], pointList[i - 1], pointList[i + 1], lineWidth / 2, true));

            uvss.Add(uvCord + Vector2.right / 3 * distance / lineWidth);
            uvss.Add(uvCord + Vector2.right / 3 * distance / lineWidth + Vector2.up / 3);
        }
        verticess.Add(pointList[pointList.Count - 1] - new Vector2(0, lineWidth / 2));
        verticess.Add(pointList[pointList.Count - 1] + new Vector2(0, lineWidth / 2));
        uvss.Add(uvList[uvList.Length - 1] + Vector2.right / 3);
        uvss.Add(uvList[uvList.Length - 1] + Vector2.one / 3);


        Vector3[] vertices = new Vector3[verticess.Count];
        Vector2[] uvs = new Vector2[uvss.Count];
        for (int i = 0; i < uvs.Length; i++)
        {
            vertices[i] = verticess[i];
            uvs[i] = uvss[i];
        }
        #endregion

        if (debugger) foreach (Vector2 uv in uvs) Debug.Log(uv);
        m.vertices = vertices;
        m.uv = uvs;
        return uvs;
    }   
        #region surface [Unused]
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
        #region old method [Unused] 
        /* int n = 0;
         int k = 0;
         for (int i = 0; i < vertices.Length; i++)
         {
             if (vertices[i] == Vector3.zero) 
             {
                 #region vertices
                 //if(i%2 == 0 && i != 0) distance += Vector2.Distance(pointList[i / 2], pointList[i / 2 - 1]) + lineWidth / Vector2.Distance(pointList[i / 2], pointList[i / 2 - 1]);
                 if (i == vertices.Length - 1 || i == 1)
                 {
                     vertices[i] = pointList[(n - 1) / 2] + new Vector2(0, lineWidth / 2);
                 }
                 else if (i == 0 || i == vertices.Length - 2)
                 {
                     vertices[i] = pointList[(n - 1) / 2] - new Vector2(0, lineWidth / 2);
                 }
                 else if (i % 2 == 0)
                 {
                     //vertices[i] = pointList[i / 2];
                     vertices[i] = Utils_Points.GetParallelePoint(pointList[n / 2], pointList[(n / 2) - 1], pointList[(n / 2) + 1], lineWidth / 2, false);
                     if (i + 2 < vertices.Length - 1)
                     {
                         vertices[i + 2] = vertices[i];
                     }
                 }
                 else
                 {
                     int pI = (n - 1) / 2;
                     vertices[i] = Utils_Points.GetParallelePoint(pointList[pI], pointList[pI - 1], pointList[pI + 1], lineWidth / 2, true);
                     if (i + 2 < vertices.Length - 1)
                     {
                         vertices[i + 2] = vertices[i];
                     }
                     //i += 2;
                 }
                 #endregion

                 #region uvs
                 if (i == 0) uvs[i] = Vector2.zero;
                 else if (i == 1) uvs[i] = Vector2.up / 3;
                 else if (i == uvs.Length - 1) uvs[i] = Vector2.one;
                 else if (i == uvs.Length - 2) uvs[i] = Vector2.right + Vector2.up * 2 / 3;
                 //else if (i == uvList.Length - 3) uvs[i] = Vector2.right * 2 / 3 + Vector2.up;
                 //else if (i == uvList.Length - 4) uvs[i] = Vector2.one * 2 / 3;
                 else if (i % 2 == 0)
                 {
                     uvCord = uvList[k % (uvList.Length - 1)];
                     uvs[i] = uvCord;

                     //Debug.Log("BottomLeft : " + uvs[i]);
                     if (i + 2 < vertices.Length - 1)
                     {
                         uvs[i + 2] = uvCord + Vector2.right / 3;
                         //Debug.Log("BottomRight : " + uvs[i + 2]);
                     }
                 }
                 else if (i % 2 == 1)
                 {
                     uvCord += Vector2.up / 3;
                     uvs[i] = uvCord;
                     //Debug.Log("UpLeft : " + uvs[i]);
                     if (i + 2 < vertices.Length - 1)
                     {
                         uvs[i + 2] = uvCord + Vector2.right / 3;
                         //Debug.Log("UpRight : " + uvs[i + 2]);
                     }
                     k++;
                 }
                 #endregion
                 n++;
             }
             //else Debug.Log(1);
         }*/
        #endregion



    public static void UpdateMeshTriangles(int pointListLength, Mesh m)
    {
        int[] triangles = new int[((3 * pointListLength) - 6)];
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
