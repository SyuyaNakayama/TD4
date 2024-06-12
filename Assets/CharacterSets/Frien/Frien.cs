using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ƒtƒŒƒ“
public class Frien : Enemy
{
    protected override void LiveEntityUpdate()
    {
        if (!IsAttacking())
        {
            SetAttackMotion("Bite");
        }
    }
}