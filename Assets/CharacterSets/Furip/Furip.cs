using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�t����
public class Furip : Enemy
{
    const float moveSpeed = 1;

    protected override void LiveEntityUpdate()
    {
        //�̗͂������Ă��鎞
        if (GetHPAmount() < 1)
        {
            //�{���
            facialExpressionName = "fury";
            //�߂��ɓG��������ڋ�
            if (GetNearestTarget() != null)
            {
                //�_��
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
                transform.Rotate(0,
                    Mathf.Atan2(-targetCursor.x, -targetCursor.z) / Mathf.Deg2Rad,
                    0, Space.Self);
                //�O�i
                Move(GetMovement() + new Vector3(0, targetCursor.y, targetCursor.z) * moveSpeed);
            }
            //��ɍU��
            if (!IsAttacking())
            {
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}