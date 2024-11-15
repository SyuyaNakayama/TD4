using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeoGroObject))]
public class Projectile : AttackArea
{
    AttackMotionData.ProjectileData projectileData;
    bool projectileDataLock = false;
    [SerializeField]
    Vector3 moveVec;
    [SerializeField]
    GeoGroObject ggobj;
    [SerializeField]
    Collider collider;

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

        ggobj.SetAllowGroundSet(projectileData.setGround);
        collider.enabled = projectileData.setGround;

        //回転角とスケーリングを考慮した方向、スピードで飛んでいく
        ggobj.SetMovement(moveVec * projectileData.speed / Time.deltaTime);

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

            if (setData.setGround && GetAttacker())
            {
                ggobj.SetCurrentGround(
                    GetAttacker().GetCurrentGround());
            }

            projectileDataLock = true;
        }
    }
}