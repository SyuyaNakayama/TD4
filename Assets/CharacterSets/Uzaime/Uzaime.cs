using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�E�U�C��
public class Uzaime : Enemy
{
    const float stickRadius = 0.5f;
    const float stickPower = 10;

    protected override void CharaUpdate()
    {
        LiveEntity[] targets = GetTargets();
        for (int i = 0; i < targets.Length; i++)
        {
            LiveEntity target = targets[i];
            if (Vector3.Magnitude(transform.InverseTransformPoint(
                target.transform.position)) <= stickRadius)
            {
                target.SetMovement(
                    target.transform.InverseTransformPoint(transform.position)
                    * stickPower);
            }
            //�U�����쒆�łȂ����Ɋl������������U�������
            else if (!IsAttacking() && GetNearestTarget() != null)
            {
                //�_��
                TargetAimY(GetNearestTarget().transform.position);
                //�U�����[�V�������Đ�
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}