using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Lift : MonoBehaviour
{
    [SerializeField]
    Switch powerSwitch;
    [SerializeField]
    float speed = 1;
    [SerializeField]
    KX_netUtil.TransformData negativeLocalTransform;
    [SerializeField]
    KX_netUtil.TransformData positiveLocalTransform;

    float progress;

    void FixedUpdate()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if(powerSwitch == null || powerSwitch.GetActive())
        {
            progress += Time.deltaTime;
        }
        float intensity = 
            KX_netUtil.RangeMap(Mathf.Cos(progress * speed),1,-1,0,1);

        if(transform.parent != null)
        {
            rigidbody.MovePosition(Vector3.Lerp(
                transform.parent.TransformPoint(negativeLocalTransform.position),
                transform.parent.TransformPoint(positiveLocalTransform.position),
                intensity));
        }
        else
        {
            rigidbody.MovePosition(Vector3.Lerp(
                negativeLocalTransform.position,
                positiveLocalTransform.position,
                intensity));
        }

        transform.localRotation =
            Quaternion.Slerp(
            Quaternion.Euler(negativeLocalTransform.eulerAngles),
            Quaternion.Euler(positiveLocalTransform.eulerAngles),
            intensity);
            
        transform.localScale =
            Vector3.Lerp(
            negativeLocalTransform.scale,
            positiveLocalTransform.scale,
            intensity);
    }
}
