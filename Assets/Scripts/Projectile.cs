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
        projectileData.lifetime--;
        if (projectileData.lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetData(AttackMotionData.ProjectileData setData)
    {
        //¶¬‚³‚ê‚½’¼Œã‚Ì‚ÝŽÀs
        if (!projectileDataLock)
        {
            projectileData = setData;
            projectileDataLock = true;
        }
    }
}