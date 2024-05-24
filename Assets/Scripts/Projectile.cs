using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : AttackArea
{
    AttackMotionData.ProjectileData projectileData;
    bool projectileDataLock = false;
    Vector3 moveVec;
    protected override void AttackAreaUpdate()
    {
        //内部変数が変更されないようにロック
        projectileDataLock = true;

        //スケーリングの基準となるオブジェクトを決める
        GameObject scalingTarget = gameObject;
        if (GetAttacker() != null)
        {
            scalingTarget = GetAttacker().gameObject;
        }
        //回転角とスケーリングを考慮した方向、スピードで飛んでいく
        transform.position += transform.rotation * moveVec
            * projectileData.speed * scalingTarget.transform.lossyScale.x;

        //寿命が尽きたら消える
        projectileData.lifetime--;
        if (projectileData.lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetProjectileData(AttackMotionData.ProjectileData setData, Vector3 setMoveVec)
    {
        //生成された直後のみ実行
        if (!projectileDataLock)
        {
            projectileData = setData;
            moveVec = setMoveVec;
            projectileDataLock = true;
        }
    }
}