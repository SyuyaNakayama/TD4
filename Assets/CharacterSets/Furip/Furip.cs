using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�t���[���b�v
public class Furip : Enemy
{
    const float moveSpeed = 1;
    const float flipIntensity = 0.1f;

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
                TargetAimY(GetNearestTarget().transform.position, flipIntensity);
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
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