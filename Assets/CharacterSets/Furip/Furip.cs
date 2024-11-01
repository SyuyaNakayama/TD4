using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//フューリップ
public class Furip : Enemy
{
    const float moveSpeed = 1;
    const float flipIntensity = 0.1f;

    protected override void LiveEntityUpdate()
    {
        //体力が減っている時
        if (GetHPAmount() < 1)
        {
            //怒り顔
            facialExpressionName = "fury";
            //近くに敵がいたら接近
            if (GetNearestTarget() != null)
            {
                //狙う
                TargetAimY(GetNearestTarget().transform.position, flipIntensity);
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
                //前進
                Move(GetMovement() + new Vector3(0, targetCursor.y, targetCursor.z) * moveSpeed);
            }
            //常に攻撃
            if (!IsAttacking())
            {
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}