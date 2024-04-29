using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//キュルフ
public class Cub_E : Enemy
{
    Vector3 targetCursor;
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
            if (GetAttackProgress() > 0.5f)
            {

            }
        }
        else
        {
            //攻撃動作中でない時に獲物を見つけたら攻撃動作へ
            if (GetNearestTarget() != null)
            {
                targetCursor = GetNearestTarget().transform.position;
                SetAttackMotion("upperAim", 60);
            }
        }
    }
}