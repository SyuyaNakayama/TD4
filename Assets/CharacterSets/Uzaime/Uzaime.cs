using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ÉEÉUÉCÉÄ
public class Uzaime : Enemy
{
    const float stickRadius = 0.5f;
    const float stickPower = 10;

    protected override void LiveEntityUpdate()
    {
        LiveEntity[] targets = GetTargets();
        for (int i = 0; i < targets.Length; i++)
        {
            LiveEntity target = targets[i];
            if (Vector3.Magnitude(transform.InverseTransformPoint(
                target.transform.position)) <= stickRadius)
            {
                target.Move(
                    target.transform.InverseTransformPoint(transform.position)
                    * stickPower);
            }
            //çUåÇìÆçÏíÜÇ≈Ç»Ç¢éûÇ…älï®Çå©Ç¬ÇØÇΩÇÁçUåÇìÆçÏÇ÷
            else if (!IsAttacking() && GetNearestTarget() != null)
            {
                //ë_Ç§
                TargetAimY(GetNearestTarget().transform.position);
                //çUåÇÉÇÅ[ÉVÉáÉìÇçƒê∂
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}