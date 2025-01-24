using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MultipleSwitchBindedFloor : MonoBehaviour
{
    [SerializeField]
    Switch[] bindSwitches;
    [SerializeField]
    LiveEntityObserver[] LiveEntityObservers;
    [SerializeField]
    KX_netUtil.TransformData negativeLocalTransform;
    [SerializeField]
    KX_netUtil.TransformData[] positiveLocalTransforms;
    [Range(0.1f, 1), SerializeField]
    float transformFlipIntensity = 0.4f;

    void FixedUpdate()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        KX_netUtil.TransformData targetLocalTransform = negativeLocalTransform;

        UInt32 switchActives = 0;
        foreach (var bindSwitch in bindSwitches)
        {
            if (bindSwitch.GetActive()) { switchActives++; }
        }
        foreach (var bindSwitch in LiveEntityObservers)
        {
            if (!bindSwitch.GetActive()) { switchActives++; }
        }

        if (switchActives > 0)
        {
            targetLocalTransform = positiveLocalTransforms[switchActives - 1];
        }

        if (transform.parent != null)
        {
            rigidbody.MovePosition(Vector3.Lerp(
                transform.parent.TransformPoint(transform.localPosition),
                transform.parent.TransformPoint(targetLocalTransform.position),
                transformFlipIntensity));
        }
        else
        {
            rigidbody.MovePosition(Vector3.Lerp(
                transform.localPosition,
                targetLocalTransform.position,
                transformFlipIntensity));
        }

        transform.localRotation =
            Quaternion.Slerp(transform.localRotation,
            Quaternion.Euler(targetLocalTransform.eulerAngles),
            transformFlipIntensity);

        transform.localScale =
            Vector3.Lerp(transform.localScale, targetLocalTransform.scale,
            transformFlipIntensity);
    }
}
