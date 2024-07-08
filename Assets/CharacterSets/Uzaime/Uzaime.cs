using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�E�U�C��
public class Uzaime : Enemy
{
    const float stickRadius = 0.5f;
    const float stickPower = 10;

    protected override void LiveEntityUpdate()
    {
        LiveEntity[] targets = GetTargets();
        for (int i = 0; i < targets.Length; i++)
        {
            LiveEntity target = targets[i];
            if (Vector3.Magnitude(transform.InverseTransformPoint(
                target.transform.position)) <= stickRadius)
            {
                target.Move(
                    target.transform.InverseTransformPoint(transform.position)
                    * stickPower);
            }
            //�U�����쒆�łȂ����Ɋl������������U�������
            else if (!IsAttacking() && GetNearestTarget() != null)
            {
                //�_��
                targetCursor = transform.InverseTransformPoint(
                    GetNearestTarget().transform.position);
                transform.Rotate(0,
                    Mathf.Atan2(targetCursor.x, targetCursor.z) / Mathf.Deg2Rad,
                    0, Space.Self);
                //�U�����[�V�������Đ�
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}