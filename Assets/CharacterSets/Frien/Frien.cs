using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�t����
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