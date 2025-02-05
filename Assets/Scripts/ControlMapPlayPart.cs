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
            int playerIndex = GetManager().GetPlayerIndex();

            jumpInput = keyMap.GetKey(playerIndex, "jump");
            weaponInput = keyMap.GetKey(playerIndex, "weapon");
            menuInput = keyMap.GetKey(playerIndex, "menu");

            moveInputVec = keyMap.GetVectorInput(playerIndex, "moveStick")
                + keyMap.GetVectorInput(playerIndex, "moveStick2");
            camInputVec = keyMap.GetVectorInput(playerIndex, "camStick")
                + keyMap.GetVectorInput(playerIndex, "camStick2");

            bool upInput = keyMap.GetKey(playerIndex, "up");
            bool downInput = keyMap.GetKey(playerIndex, "down");
            bool rightInput = keyMap.GetKey(playerIndex, "right");
            bool leftInput = keyMap.GetKey(playerIndex, "left");

            bool camUpInput = keyMap.GetKey(playerIndex, "camUp");
            bool camDownInput = keyMap.GetKey(playerIndex, "camDown");
            bool camRightInput = keyMap.GetKey(playerIndex, "camRight");
            bool camLeftInput = keyMap.GetKey(playerIndex, "camLeft");

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