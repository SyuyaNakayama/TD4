using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//フレン
public class Frien : Enemy
{
    const float moveSpeed = 1;

    bool awake;

    protected override void LiveEntityUpdate()
    {
        //まず周囲にいる見方をサーチ
        LiveEntity[] friends = GetFriends();
        //サーチした味方の内一体でも死んでいたら目を覚ます
        for (int i = 0; i < friends.Length; i++)
        {
            if (!friends[i].IsLive())
            {
                awake = true;
                break;
            }
        }

        //目を覚ましている時
        if(awake)
        {
            //近くに敵がいたら接近
            if(GetNearestTarget() != null)
            {
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
                Move(GetMovement() + targetCursor * moveSpeed);
            }
            //常に攻撃
            if (!IsAttacking())
            {
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}