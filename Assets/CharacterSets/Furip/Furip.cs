using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ƒtƒŒƒ“
public class Furip : Enemy
{
    const float moveSpeed = 1;

    protected override void LiveEntityUpdate()
    {
        //‘Ì—Í‚ªŒ¸‚Á‚Ä‚¢‚éŽž
        if (GetHPAmount() < 1)
        {
            //“{‚èŠç
            facialExpressionName = "fury";
            //‹ß‚­‚É“G‚ª‚¢‚½‚çÚ‹ß
            if (GetNearestTarget() != null)
            {
                //‘_‚¤
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
                transform.Rotate(0,
                    Mathf.Atan2(-targetCursor.x, -targetCursor.z) / Mathf.Deg2Rad,
                    0, Space.Self);
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