using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : AttackArea
{
    AttackMotionData.ProjectileData projectileData;
    bool projectileDataLock = false;
    protected override void AttackAreaUpdate()
    {
        projectileDataLock = true;
    }

    public void SetData(AttackMotionData.ProjectileData setData)
    {
        //生成された直後のみ実行
        if (!projectileDataLock)
        {
            projectileData = setData;
            projectileDataLock = true;
        }
    }
}