using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LinerLoop : MonoBehaviour
{
    [System.Serializable]
    public struct TransAndOffset
    {
        public Transform transform;
        public float offset;
    }

    [SerializeField]
    TransAndOffset[] transAndOffsets;
    [SerializeField]
    Vector3[] vertices;
    [SerializeField]
    int oneLoopFrame = 120;

    int progressFrame;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for(int i = 0;i < vertices.Length;i++)
        {
            Handles.Label(transform.TransformPoint(vertices[i]), i.ToString());
        }

        Vector3[] worldVertices = CopyArray<Vector3>(vertices);
        for(int i = 0;i < worldVertices.Length;i++)
        {
            worldVertices[i] = transform.TransformPoint(worldVertices[i]);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawLineStrip(worldVertices, true);
    }
#endif

    void FixedUpdate()
    {
        Vector3[] outPutVertices = CopyArray<Vector3>(vertices);
        Array.Resize(ref outPutVertices, outPutVertices.Length + 1);
        outPutVertices[outPutVertices.Length - 1] = outPutVertices[0];

        for (int i = 0; i < transAndOffsets.Length; i++)
        {
            transAndOffsets[i].transform.position =
                transform.TransformPoint(BrokenLineProgressedPos(
                outPutVertices, Mathf.Repeat(
                (float)progressFrame / oneLoopFrame
                + transAndOffsets[i].offset, 1)));
        }

        progressFrame++;
    }

    public Vector3 BrokenLineProgressedPos(Vector3[] indices, float progress)
    {
        float lineLength = 0;
        for (int i = 0; i < indices.Length - 1; i++)
        {
            float segmentLength = Vector3.Magnitude(indices[i + 1] - indices[i]);
            lineLength += segmentLength;
        }

        float lineProgress = lineLength * progress;
        for (int i = 0; i < indices.Length - 1; i++)
        {
            float segmentLength = Vector3.Magnitude(indices[i + 1] - indices[i]);
            if (segmentLength < lineProgress)
            {
                lineProgress -= segmentLength;
            }
            else
            {
                return Vector3.Lerp(indices[i], indices[i + 1], lineProgress / segmentLength);
            }
        }
        return indices[indices.Length - 1];
    }

    public static T[] CopyArray<T>(T[] array)
    {
        T[] ret = new T[array.Length];
        Array.Copy(array, ret, array.Length);
        return ret;
    }
}
