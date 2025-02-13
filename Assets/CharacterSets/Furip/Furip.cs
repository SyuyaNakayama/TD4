using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//?申t?申?申?申[?申?申?申b?申v
public class Furip : Enemy
{
    const float moveSpeed = 1;
    const float flipIntensity = 0.1f;

    protected override void CharaUpdate()
    {
        //?申����鐃�?申?申?申?申?申��鐃�?申��?
        if (GetLiveEntity().GetLife() < 1)
        {
            //?申��鐃�?申��G?申?申?申?申?申?申?申?申��鐃�
            if (GetNearestTarget() != null)
            {
                //?申_?申?申
                TargetAimY(GetNearestTarget().transform.position, flipIntensity);
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
                //?申O?申i
                GetLiveEntity().SetMovement(GetLiveEntity().GetMovement() + new Vector3(0, targetCursor.y, targetCursor.z) * moveSpeed);
            }
            //?申?申��U?申?申
            if (!IsAttacking())
            {
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}