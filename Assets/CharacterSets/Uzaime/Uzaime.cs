using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//?øΩE?øΩU?øΩC?øΩ?øΩ
public class Uzaime : Enemy
{
    const float stickRadius = 0.5f;
    const float stickPower = 10;

    protected override void CharaUpdate()
    {
        if (GetNearestTarget() != null)
        {
            //?øΩ_?øΩ?øΩ
            TargetAimY(GetNearestTarget().transform.position);
            //?øΩU?øΩ?øΩ?øΩ?øΩ?øΩ[?øΩV?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩƒêÔøΩ
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
            //?øΩU?øΩ?øΩ?øΩ?øΩ?øΩ?íÜ?øΩ≈Ç»ÇÔøΩ?øΩ?øΩ?øΩ…äl?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ¬ÇÔøΩ?øΩ?øΩ?øΩ?øΩU?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ
            else if (!IsAttacking() && GetNearestTarget() != null)
            {
                //?øΩ_?øΩ?øΩ
                TargetAimY(GetNearestTarget().transform.position);
                //?øΩU?øΩ?øΩ?øΩ?øΩ?øΩ[?øΩV?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩƒêÔøΩ
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}