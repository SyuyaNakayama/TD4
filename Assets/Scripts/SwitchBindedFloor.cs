using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBindedFloor : MonoBehaviour
{
    const float transformFlipIntensity = 0.4f;

    [SerializeField]
    Switch bindSwitch;
    [SerializeField]
    Vector3 negativeLocalPosition;
    [SerializeField]
    Vector3 negativeLocalEulerAngles;
    [SerializeField]
    Vector3 negativeLocalScale;
    [SerializeField]
    Vector3 positiveLocalPosition;
    [SerializeField]
    Vector3 positiveLocalEulerAngles;
    [SerializeField]
    Vector3 positiveLocalScale;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetLocalPosition = negativeLocalPosition;
        Vector3 targetLocalEulerAngles = negativeLocalEulerAngles;
        Vector3 targetLocalScale = negativeLocalScale;
        if(bindSwitch.GetActive())
        {
            targetLocalPosition = positiveLocalPosition;
            targetLocalEulerAngles = positiveLocalEulerAngles;
            targetLocalScale = positiveLocalScale;
        }

        transform.localPosition =
            Vector3.Lerp(transform.localPosition,targetLocalPosition,
            transformFlipIntensity);
        transform.localRotation =
            Quaternion.Slerp(transform.localRotation,
            Quaternion.Euler(targetLocalEulerAngles),
            transformFlipIntensity);
        transform.localScale =
            Vector3.Lerp(transform.localScale,targetLocalScale,
            transformFlipIntensity);
    }
}
