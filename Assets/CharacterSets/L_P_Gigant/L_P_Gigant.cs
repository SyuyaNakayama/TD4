using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_P_Gigant : Boss
{
    const float flipIntensitySoft = 0.01f;
    const float flipIntensityHard = 0.1f;

    protected override void BossUpdate()
    {
        if (IsUniqueActing("softAim"))
        {
            TargetChange();
            TargetAimY(targetCursor, flipIntensitySoft);
        }
        if (IsUniqueActing("hardAim"))
        {
            TargetChange();
            TargetAimY(targetCursor, flipIntensityHard);
        }
    }
}