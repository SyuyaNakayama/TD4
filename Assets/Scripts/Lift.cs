using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField]
    float speed = 1;
    [SerializeField]
    KX_netUtil.TransformData negativeLocalTransform;
    [SerializeField]
    KX_netUtil.TransformData positiveLocalTransform;

    float progress;

    void FixedUpdate()
    {
        progress += Time.deltaTime;
        float intensity = 
            KX_netUtil.RangeMap(Mathf.Sin(progress * speed),-1,1,0,1);

        transform.localPosition =
            Vector3.Lerp(
            negativeLocalTransform.position,
            positiveLocalTransform.position,
            intensity);

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
