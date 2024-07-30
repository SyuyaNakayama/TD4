using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRenderer_Local : MonoBehaviour
{
    public Transform parentTransform;

    private LineRenderer myLine;
    Vector3[] points = { };

    public float distIncrement = 0.1f;
    private Vector3 lastPosition;

    public bool inverse = true;
    public bool limitTrailLength = false;
    public int maxPositions = 10;

    void Awake()
    {
        myLine = GetComponent<LineRenderer>();
        myLine.useWorldSpace = false;
        Reset();
    }

    void OnEnable()
    {
        Reset();
    }

    void Reset()
    {
        Array.Resize(ref points, 0);
    }

    void AddPoint(Vector3 newPoint)
    {
        Array.Resize(ref points, points.Length + 1);
        points[points.Length - 1] = newPoint;

        lastPosition = newPoint;
    }

    void TruncatePositions(int newLength)
    {
        if (limitTrailLength && newLength > maxPositions)
        {
            newLength = maxPositions;
        }
        Vector3[] tempList = new Vector3[newLength];
        int nExtraItems = points.Length - newLength;
        for (int i = 0; i < newLength; i++)
        {
            tempList[i] = points[i + nExtraItems];
            tempList[i] = transform.InverseTransformPoint(parentTransform.TransformPoint(tempList[i]));
        }
        if (inverse)
        {
            Vector3[] tempList2 = new Vector3[newLength];
            for (int i = 0; i < newLength; i++)
            {
                tempList2[i] = tempList[tempList.Length - 1 - i];
            }
            tempList = tempList2;
        }
        if (myLine != null)
        {
            myLine.positionCount = newLength;
            myLine.SetPositions(tempList);
        }
    }

    void FixedUpdate()
    {
        Vector3 curPosition = parentTransform.InverseTransformPoint(transform.position);
        if (Vector3.Distance(curPosition, lastPosition) > distIncrement)
        {
            AddPoint(curPosition);
        }
    }

    void OnWillRenderObject()
    {
        TruncatePositions(points.Length);
    }
}
