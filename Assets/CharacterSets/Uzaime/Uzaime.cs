using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//?申E?申U?申C?申?申
public class Uzaime : Enemy
{
    const float stickRadius = 0.5f;
    const float stickPower = 10;

    protected override void CharaUpdate()
    {
        if (GetNearestTarget() != null)
        {
            //?申_?申?申
            TargetAimY(GetNearestTarget().transform.position);
            //?申U?申?申?申?申?申[?申V?申?申?申?申?申?申?申��鐃�
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }

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
            //?申U?申?申?申?申?申?��?申����鐃�?申?申?申��l?申?申?申?申?申?申?申��鐃�?申?申?申?申U?申?申?申?申?申?申?申
            else if (!IsAttacking() && GetNearestTarget() != null)
            {
                //?申_?申?申
                TargetAimY(GetNearestTarget().transform.position);
                //?申U?申?申?申?申?申[?申V?申?申?申?申?申?申?申��鐃�
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}