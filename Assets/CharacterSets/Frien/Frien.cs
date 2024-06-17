using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ƒtƒŒƒ“
public class Frien : Enemy
{
    const int maxAwakeTimeframe = 600;

    int awakeTimeFrame;

    protected override void LiveEntityUpdate()
    {
        if(awakeTimeFrame > 0)
        {
            if (!IsAttacking())
            {
                SetAttackMotion("Bite");
            }
        }
    }
}