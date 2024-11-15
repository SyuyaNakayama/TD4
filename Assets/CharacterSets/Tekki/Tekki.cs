using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//エネミィ
public class Tekki : Enemy
{
    const int StackWallDetectFrame = 2;
    const float walkSpeed = 1;

    float stackWall;

    protected override void CharaUpdate()
    {
        TurnAtTheWall(new Vector3(0, 180, 0), 0.001f);

        GetLiveEntity().SetMovement(new Vector3(GetLiveEntity().GetMovement().x, GetLiveEntity().GetMovement().y, walkSpeed));
        if (!IsAttacking())
        {
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }
    }

    protected void TurnAtTheWall(Vector3 turnEuler, float graceRange)
    {
        if (Mathf.Abs(Vector3.Dot(new Vector3(0, 0, 1), GetLiveEntity().GetPushBackVec())) > graceRange)
        {
            stackWall++;
        }
        else
        {
            stackWall--;
        }
        if (stackWall >= StackWallDetectFrame)
        {
            GetLiveEntity().transform.Rotate(turnEuler, Space.Self);
            stackWall = 0;
        }

        stackWall = Mathf.Clamp(stackWall, 0, StackWallDetectFrame);
    }
}
