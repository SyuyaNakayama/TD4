using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

public class Player : CharacterCassette
{
    public const int maxTeamNum = 5;
    public const int maxAttackReactionframe = 10;
    const float cameraControlSpeed = 3;
    const float moveSpeed = 1.5f;
    const float jumpPower = 10.0f;
    const float maxPlayerRotSpeed = 3;
    const float playerRotSpeedDiffuse = 0.5f;

    bool prevJumpInput;
    bool attackTrigger;
    int attackReactionFrame;
    public int GetAttackReactionFrame()
    {
        return attackReactionFrame;
    }
    int currentCharaIndex;
    public int GetCurrentCharaIndex()
    {
        return currentCharaIndex;
    }
    [SerializeField]
    CharaData[] weapons;
    public CharaData[] GetWeapons()
    {
        return weapons;
    }
    float playerRotSpeed;

    protected override void CharaUpdate()
    {
        List<CharaData> weaponList = new List<CharaData>(weapons);
        weaponList.Remove(null);
        weapons = weaponList.ToArray();

        //移動用の方向入力で移動
        Vector2 moveInputVec = GetLiveEntity().GetControlMap().GetMoveInputVec();
        if (moveInputVec.x != 0 || moveInputVec.y != 0)
        {
            GetLiveEntity().SetMovement(GetLiveEntity().GetMovement() + new Vector3(
                moveInputVec.x,
                0,
                moveInputVec.y).normalized
                * moveSpeed);
            GetLiveEntity().SetDirection(Mathf.Atan2(
                moveInputVec.x, moveInputVec.y)
                / Mathf.Deg2Rad);
        }

        //ジャンプの入力でジャンプ
        bool jumpInput = GetLiveEntity().GetControlMap().GetJumpInput();
        if (jumpInput && !prevJumpInput)
        {
            GetLiveEntity().SetMovement(new Vector3(
                GetLiveEntity().GetMovement().x,
                jumpPower,
                GetLiveEntity().GetMovement().z));
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }
        prevJumpInput = jumpInput;

        attackReactionFrame = Mathf.Max(0, attackReactionFrame - 1);

        //攻撃の入力で攻撃
        bool attackInput = GetLiveEntity().GetControlMap().GetWeaponInput();
        if (attackInput && !attackTrigger)
        {
            attackReactionFrame = maxAttackReactionframe;
            if (weapons.Length > 0)
            {
                SetAttackMotion(
                    weapons[currentCharaIndex].SearchAttackMotion(
                    weapons[currentCharaIndex].GetWeaponedAttackMotionName()));
                currentCharaIndex++;
            }
        }
        attackTrigger = attackInput;
        currentCharaIndex = Mathf.RoundToInt(Mathf.Repeat(currentCharaIndex,
            weapons.Length));

        //武器使用中のアニメーション
        if (IsAttacking() && !IsAttacking(GetData().GetDefaultAttackMotionName()))
        {
            GetVisual().animationName = "attack";
        }

        //カメラ用の上下入力でカメラの仰角調整
        Vector2 camInputVec = GetLiveEntity().GetControlMap().GetCamInputVec();
        transform.Rotate(
            0, camInputVec.x * cameraControlSpeed, 0, Space.Self);
        float playerCameraAngle =
            GetLiveEntity().GetCameraAngle() + camInputVec.y * cameraControlSpeed;

        playerCameraAngle = Mathf.Clamp(
            playerCameraAngle, LiveEntity.MinCameraAngle, LiveEntity.MaxCameraAngle);
        GetLiveEntity().SetCameraAngle(playerCameraAngle);
        //カメラ用の左右入力で回転
        playerRotSpeed = Mathf.Clamp(
            playerRotSpeed * playerRotSpeedDiffuse + camInputVec.x,
            -maxPlayerRotSpeed, maxPlayerRotSpeed);
        GetLiveEntity().transform.Rotate(0, playerRotSpeed, 0, Space.Self);
        //時期は逆回転し、回転を相殺
        GetLiveEntity().SetDirection(
            GetLiveEntity().GetDirection() - playerRotSpeed);
    }

    public void EquipCharacter(CharaData charaData)
    {
        if (GetLiveEntity().GetAllowedItemEffect())
        {
            if (weapons.Length < maxTeamNum)
            {
                List<CharaData> list = new List<CharaData>(weapons);
                list.Insert(Mathf.Clamp(currentCharaIndex, 0, weapons.Length),
                    charaData);
                weapons = list.ToArray();
            }
            else
            {
                weapons[currentCharaIndex] = charaData;
            }
        }
    }
}