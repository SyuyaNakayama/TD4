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
        projectileDataLock = true;

        GameObject scalingTarget = gameObject;
        if (GetAttacker() != null)
        {
            scalingTarget = GetAttacker().gameObject;
        }
        transform.position += transform.rotation * moveVec
            * projectileData.speed * scalingTarget.transform.lossyScale.x;

        projectileData.lifetime--;
        if (projectileData.lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetProjectileData(AttackMotionData.ProjectileData setData, Vector3 setMoveVec)
    {
        //¶¬‚³‚ê‚½’¼Œã‚Ì‚ÝŽÀs
        if (!projectileDataLock)
        {
            projectileData = setData;
            moveVec = setMoveVec;
            projectileDataLock = true;
        }
    }
}