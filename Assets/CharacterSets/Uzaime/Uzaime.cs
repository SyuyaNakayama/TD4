using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ウザイム
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
            //攻撃動作中でない時に獲物を見つけたら攻撃動作へ
            else if (!IsAttacking() && GetNearestTarget() != null)
            {
                //狙う
                targetCursor = transform.InverseTransformPoint(
                    GetNearestTarget().transform.position);
                transform.Rotate(0,
                    Mathf.Atan2(targetCursor.x, targetCursor.z) / Mathf.Deg2Rad,
                    0, Space.Self);
                //攻撃モーションを再生
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}