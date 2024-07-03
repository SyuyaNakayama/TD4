using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotLift : MonoBehaviour
{
    [SerializeField]
    Vector3 rotateEuler;

    float progress;

    void FixedUpdate()
    {
        progress += Time.deltaTime;
        transform.localEulerAngles = rotateEuler * progress;
    }
}
