using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Utils_Points
{
    public static float[] GeneratePointArray(float[] pointArray, float lineStart, float lineEnd, float lineResolution)
    {
        int pointArrayLength = Mathf.CeilToInt((lineEnd - lineStart) / lineResolution);
        pointArray = new float[pointArrayLength];


        for (int i = 0; i < pointArray.Length; i++)
        {
            pointArray[i] = lineStart + (i * lineResolution);
        }
        pointArray[pointArray.Length - 1] = lineEnd;
        return pointArray;
    }

    public static int CurrentListIndex(float[] pointArray, List<List<Vector2>> lineList, Vector2 currentPosition, float maxDistance, Action AddPointCall = null)
    {
        if (lineList.Count < 1)
        {
            AddLineList(pointArray, lineList, currentPosition, AddPointCall);
            return lineList.Count - 1;
        }

        for (int i = 0; i < lineList.Count; i++)
        {
            for (int j = 0; j < lineList[i].Count; j++)
            {
                float distance = Mathf.Abs(lineList[i][j].x - currentPosition.x);
                if (distance < maxDistance) return i;
            }
        }

        AddLineList(pointArray, lineList, currentPosition, AddPointCall);
        return lineList.Count - 1;
    }

    static private void AddLineList(float[] pointArray, List<List<Vector2>> lineList, Vector2 currentPosition, Action AddPointCall = null)
    {
        lineList.Add(new List<Vector2>());
        Vector2 pointPos = new Vector2(closestPoint(pointArray, currentPosition.x), currentPosition.y);
        lineList[lineList.Count - 1].Add(pointPos);
        AddPointCall?.Invoke();
    }

    public static float closestPoint(float[] pointArray, float positionX)
    {
        float pointX = 0;
        float distance = 1000;
        foreach (float point in pointArray)
        {
            float tempDistance = Mathf.Abs(positionX - point);

            if (tempDistance < distance)
            {
                pointX = point;
                distance = tempDistance;
            }
        }
        return pointX;
    }

    public static int ClosestPointInList(List<Vector2> pointList, float currentPositionX, float[] pointArray)
    {
        int pointX = 0;
        float distance = 1000;
        for (int i = 0; i < pointList.Count; i++)
        {
            float tempDistance = Mathf.Abs(currentPositionX - pointList[i].x);

            if (tempDistance < distance)
            {
                pointX = i;
                distance = tempDistance;
            }
        }
        return pointX;
    }

    public static float ClosestFloatPointInList(List<Vector2> pointList, float currentPositionX, float[] pointArray)
    {
        int pointX = 0;
        float distance = 1000;
        for (int i = 0; i < pointList.Count; i++)
        {
            float tempDistance = Mathf.Abs(currentPositionX - pointList[i].x);

            if (tempDistance < distance)
            {
                pointX = i;
                distance = tempDistance;
            }
        }
        return pointList[pointX].x;
    }


    public static int AddPoints(float[] pointArray, List<Vector2> pointList, float closestVertice, Vector2 currentPosition, float minDistance, float offSetY)
    {
        float playerPointX = closestPoint(pointArray, currentPosition.x);
        float temp = Mathf.Abs(playerPointX - closestVertice);
        float prevY = pointList[pointList.Count - 1].y;
        int numOfPointsAdded = 0;
        if (playerPointX - closestVertice < 0)
        {
            for (float i = temp - minDistance; i > 0; i -= minDistance)
            {
                float xPos = playerPointX + i;
                float yPos = currentPosition.y - ((currentPosition.y - prevY) * i / temp);
                Vector2 newPoint = new Vector2(xPos, yPos);
                pointList.Add(newPoint);
                numOfPointsAdded++;
                //charC.transform.localScale -= Vector3.one * charC.movementScaler / 100;
            }
        }
        else
        {
            for (float i = temp - minDistance; i > 0; i -= minDistance)
            {
                float xPos = playerPointX - i;
                float yPos = currentPosition.y - ((currentPosition.y - prevY) * i / temp);
                Vector2 newPoint = new Vector2(xPos, yPos);
                pointList.Add(newPoint);
                numOfPointsAdded++;

                //charC.transform.localScale -= Vector3.one * charC.movementScaler / 100;
            }
        }
        pointList.Add(new Vector2(playerPointX, currentPosition.y));
        numOfPointsAdded++;
        
        return numOfPointsAdded;
        //charC.transform.localScale -= Vector3.one * charC.movementScaler / 100;

    }

    public static void UpdatePointsPos(List<Vector2> pointList, int closestPointIndex, Vector2 currentPosition, float offSetY)
    {
        pointList[closestPointIndex] = new Vector2(pointList[closestPointIndex].x, currentPosition.y - offSetY);
    }

    public static void UpdatePoints(float[] pointArray, List<Vector2> pointList, float closestVertice, Vector2 currentPosition, float minDistance, float offSetY, Vector2 prevPos)
    {
        float playerPointX = closestPoint(pointArray, currentPosition.x);
        float prevPosX = closestPoint(pointArray, prevPos.x);
        float temp = Mathf.Abs(playerPointX - prevPosX);
        float prevY = pointList[ClosestPointInList(pointList, prevPosX, pointArray)].y;
        
        if (playerPointX - prevPosX < 0)
        {
            for (float i = temp - minDistance; i > 0; i -= minDistance)
            {
                float xPos = playerPointX + i;
                float yPos = currentPosition.y - ((currentPosition.y - prevY) * i / temp);
                Vector2 nPoint = new Vector2(xPos, yPos);
                pointList[ClosestPointInList(pointList, xPos, pointArray)] = nPoint;
            }
        }
        else
        {
            for (float i = temp - minDistance; i > 0; i -= minDistance)
            {
                float xPos = playerPointX - i;
                float yPos = currentPosition.y - ((currentPosition.y - prevY) * i / temp);
                Vector2 nPoint = new Vector2(xPos, yPos);
                pointList[ClosestPointInList(pointList, xPos, pointArray)] = nPoint;
            }
        }
        Vector2 nePoint = new Vector2(playerPointX, currentPosition.y);
        pointList[ClosestPointInList(pointList, playerPointX, pointArray)] = nePoint;


    }

    public static Vector2 GetParallelePoint(Vector2 pCentre, Vector2 pGauche, Vector2 pDroite, float distance)
    {
        Vector2 dirGaucheCentre = (pGauche - pCentre).normalized;
        Vector2 dirCentreDroite = (pCentre - pDroite).normalized;
        Vector2 aDir = Vector2.Lerp(dirGaucheCentre, dirCentreDroite, .5f).normalized;
        Vector2 cDir = Vector3.Cross(aDir, Vector3.forward);


        float angle = Vector3.Angle(dirGaucheCentre, dirCentreDroite) * Mathf.Deg2Rad * 0.5f;
        if (angle < Mathf.PI * 0.25f)
        {
            angle = Mathf.PI * 0.5f - angle;
        }
        float width = distance / Mathf.Max(Mathf.Epsilon, Mathf.Sin(angle));
        
        
        Vector2 newPoint = pCentre + (cDir * width);
        Debug.DrawLine(pCentre, newPoint, Color.white, Time.deltaTime);
        return newPoint;
    }

    public static Vector2 GetSurfacePoint(Vector2 point)
    {
        var newPos = Vector2.zero;
        RaycastHit2D hit = Physics2D.Raycast(point, -Vector2.up, Mathf.Infinity, LayerMask.GetMask("Ground"));
        if (hit) newPos = hit.point;
        return newPos;

    }
}
