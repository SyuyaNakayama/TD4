using System;
using UnityEngine;

public class AudioMappingTransform : MonoBehaviour
{
    [Serializable]
    struct TransformLerpData
    {
        public Vector2 keyFrame;
        public Transform transform;
        public KX_netUtil.TransformData localTransformData0;
        public KX_netUtil.TransformData localTransformData1;
        public KX_netUtil.EaseData easeData;
    }

    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    TransformLerpData[] transformLerpDatas = { };

    void FixedUpdate()
    {
        float progress = audioSource.time;
        for (int i = 0; i < transformLerpDatas.Length; i++)
        {
            TransformLerpData current = transformLerpDatas[i];

            float clampedProgress =
                KX_netUtil.Ease(KX_netUtil.RangeMap(progress,
                current.keyFrame.x, current.keyFrame.y, 0, 1),
                current.easeData.type, current.easeData.pow);

            current.transform.localPosition =
                Vector3.Lerp(
                current.localTransformData0.position,
                current.localTransformData1.position,
                clampedProgress);

            current.transform.localRotation =
                Quaternion.Slerp(
                Quaternion.Euler(current.localTransformData0.eulerAngles),
                Quaternion.Euler(current.localTransformData1.eulerAngles),
                clampedProgress);

            current.transform.localScale =
                Vector3.Lerp(
                current.localTransformData0.scale,
                current.localTransformData1.scale,
                clampedProgress);
        }
    }
}
