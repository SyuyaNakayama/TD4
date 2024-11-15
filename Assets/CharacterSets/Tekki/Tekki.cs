using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�G�l�~�B
public class Tekki : Enemy
{
    const float walkSpeed = 1;

    protected override void CharaUpdate()
    {
        GetLiveEntity().SetMovement(new Vector3(GetLiveEntity().GetMovement().x, GetLiveEntity().GetMovement().y, walkSpeed));
        if (!IsAttacking())
        {
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }
    }
}
