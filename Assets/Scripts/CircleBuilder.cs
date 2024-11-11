using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]

public class CircleBuilder : MonoBehaviour
{
    public float radius = 1;
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            float turnAngle;
            turnAngle = i / (float)transform.childCount * 2 * Mathf.PI;
            transform.GetChild(i).localPosition = new Vector3(-Mathf.Sin(turnAngle) * radius, transform.GetChild(i).localPosition.y, Mathf.Cos(turnAngle) * radius);
        }
    }
}
