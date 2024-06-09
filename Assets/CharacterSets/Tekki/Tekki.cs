using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�G�l�~�B
public class Tekki : Enemy
{
    const float walkSpeed = 1;

    protected override void LiveEntityUpdate()
    {
        Move(new Vector3(GetMovement().x,GetMovement().y,walkSpeed));
        if (!IsAttacking())
        {
            SetAttackMotion("spikeBody");
        }
    }
}
