using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//?��t?��?��?��[?��?��?��b?��v
public class Furip : Enemy
{
    const float moveSpeed = 1;
    const float flipIntensity = 0.1f;

    protected override void CharaUpdate()
    {
        //?��̗͂�?��?��?��?��?��Ă�?���?
        if (GetLiveEntity().GetLife() < 1)
        {
            //?��߂�?��ɓG?��?��?��?��?��?��?��?��ڋ�
            if (GetNearestTarget() != null)
            {
                //?��_?��?��
                TargetAimY(GetNearestTarget().transform.position, flipIntensity);
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
                //?��O?��i
                GetLiveEntity().SetMovement(GetLiveEntity().GetMovement() + new Vector3(0, targetCursor.y, targetCursor.z) * moveSpeed);
            }
            //?��?��ɍU?��?��
            if (!IsAttacking())
            {
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}