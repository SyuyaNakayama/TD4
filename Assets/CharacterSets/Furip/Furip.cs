using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ƒtƒ…[ƒŠƒbƒv
public class Furip : Enemy
{
    const float moveSpeed = 1;
    const float flipIntensity = 0.1f;

    protected override void LiveEntityUpdate()
    {
        //‘Ì—Í‚ªŒ¸‚Á‚Ä‚¢‚é
        if (GetHPAmount() < 1)
        {
            //“{‚èŠç
            facialExpressionName = "fury";
            //‹ß‚­‚É“G‚ª‚¢‚½‚çÚ‹ß
            if (GetNearestTarget() != null)
            {
                //‘_‚¤
                TargetAimY(GetNearestTarget().transform.position, flipIntensity);
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
                //‘Oi
                Move(GetMovement() + new Vector3(0, targetCursor.y, targetCursor.z) * moveSpeed);
            }
            //í‚ÉUŒ‚
            if (!IsAttacking())
            {
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}