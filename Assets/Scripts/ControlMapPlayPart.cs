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
            KeyMap keyMap = GetManager().GetKeyMap();

            jumpInput = keyMap.GetKey("jump");
            weaponInput = keyMap.GetKey("weapon");
            menuInput = keyMap.GetKey("menu");

            moveInputVec = keyMap.GetVectorInput("moveStick")
                + keyMap.GetVectorInput("moveStick2");
            camInputVec = keyMap.GetVectorInput("camStick")
                + keyMap.GetVectorInput("camStick2");

            bool upInput = keyMap.GetKey("up");
            bool downInput = keyMap.GetKey("down");
            bool rightInput = keyMap.GetKey("right");
            bool leftInput = keyMap.GetKey("left");

            bool camUpInput = keyMap.GetKey("camUp");
            bool camDownInput = keyMap.GetKey("camDown");
            bool camRightInput = keyMap.GetKey("camRight");
            bool camLeftInput = keyMap.GetKey("camLeft");

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