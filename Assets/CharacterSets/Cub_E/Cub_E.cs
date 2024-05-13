using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//キュルフ
public class Cub_E : Enemy
{
    protected override void LiveEntityUpdate()
    {
        //y軸には空気抵抗がかからないように設定
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;
        //重力を強めに設定
        gravityScale = 1;

        if (IsAttacking())
        {
            if (GetAttackProgress() < 0.5f)
            {
                //標的の上に移動
                Vector3 target = targetCursor
                    + transform.TransformPoint(new Vector3(0, 3, 0))
                    - transform.position;
                movement = transform.InverseTransformPoint(target)
                    / Mathf.Deg2Rad * 0.1f;
            }
        }
        else
        {
            //攻撃動作中でない時に獲物を見つけたら攻撃動作へ
            if (GetNearestTarget() != null)
            {
                targetCursor = GetNearestTarget().transform.position;
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}