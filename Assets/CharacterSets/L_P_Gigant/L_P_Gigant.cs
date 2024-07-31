using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_P_Gigant : Boss
{
    protected override void BossUpdate()
    {
        if (IsAttacking("gigantAccelerator"))
        {
            TargetChange();
            TargetAimY(targetCursor, 0.01f);
        }
        if (IsAttacking("gigantSaw"))
        {
            TargetChange();
            TargetAimY(targetCursor, 0.1f);
        }
    }
}
