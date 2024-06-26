using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//フレン
public class Furip : Enemy
{
    const float moveSpeed = 1;

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
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
                transform.Rotate(0,
                    Mathf.Atan2(-targetCursor.x, -targetCursor.z) / Mathf.Deg2Rad,
                    0, Space.Self);
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