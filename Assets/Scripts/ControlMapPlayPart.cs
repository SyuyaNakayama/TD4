using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMapPlayPart : ControlMap
{
    bool jumpInput;
    public bool GetJumpInput()
    {
        return jumpInput;
    }
    bool weaponInput;
    public bool GetWeaponInput()
    {
        return weaponInput;
    }
    bool menuInput;
    public bool GetMenuInput()
    {
        return menuInput;
    }
    Vector2 moveInputVec;
    public Vector2 GetMoveInputVec()
    {
        return moveInputVec;
    }
    Vector2 camInputVec;
    public Vector2 GetCamInputVec()
    {
        return camInputVec;
    }

    protected override void ControlMapUpdate()
    {
        if (IsUserControl())
        {
            jumpInput = GetKeyMap().GetKey("jump");
            weaponInput = GetKeyMap().GetKey("weapon");
            menuInput = GetKeyMap().GetKey("menu");

            moveInputVec = GetKeyMap().GetVectorInput("moveStick")
                + GetKeyMap().GetVectorInput("moveStick2");
            camInputVec = GetKeyMap().GetVectorInput("camStick")
                + GetKeyMap().GetVectorInput("camStick2");

            bool upInput = GetKeyMap().GetKey("up");
            bool downInput = GetKeyMap().GetKey("down");
            bool rightInput = GetKeyMap().GetKey("right");
            bool leftInput = GetKeyMap().GetKey("left");

            bool camUpInput = GetKeyMap().GetKey("camUp");
            bool camDownInput = GetKeyMap().GetKey("camDown");
            bool camRightInput = GetKeyMap().GetKey("camRight");
            bool camLeftInput = GetKeyMap().GetKey("camLeft");

            moveInputVec +=
                DPadToVector2(upInput, downInput, rightInput, leftInput);
            if (moveInputVec.magnitude > 1)
            {
                moveInputVec = moveInputVec.normalized;
            }

            camInputVec +=
                DPadToVector2(camUpInput, camDownInput, camRightInput, camLeftInput);
            if (camInputVec.magnitude > 1)
            {
                camInputVec = camInputVec.normalized;
            }
        }
    }

    Vector2 DPadToVector2(
        bool upInput, bool downInput, bool rightInput, bool leftInput)
    {
        Vector2 ret = Vector2.zero;
        if (upInput)
        {
            ret.y += 1;
        }
        if (downInput)
        {
            ret.y -= 1;
        }
        if (rightInput)
        {
            ret.x += 1;
        }
        if (leftInput)
        {
            ret.x -= 1;
        }

        return ret;
    }
}