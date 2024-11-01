using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

public class Player : LiveEntity
{
    public const int maxTeamNum = 5;
    public const int maxAttackReactionframe = 10;
    const float cameraControlSpeed = 3;
    const float moveSpeed = 1.5f;
    const float jumpPower = 10.0f;

    bool jumpTrigger;
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
    CharaData[] characters;
    public CharaData[] GetCharacters()
    {
        return characters;
    }
    float playerCameraAngle = maxCameraAngle;
    bool allowedItemEffect;
    Item[] touchedItems = { };

    protected override void LiveEntityUpdate()
    {
        //キャラクターの配列からnullを消す
        List<CharaData> characterList = new List<CharaData>(characters);
        characterList.Remove(null);
        characters = characterList.ToArray();

        //接触したアイテムの配列からnullを消す
        List<Item> touchedItemList = new List<Item>(touchedItems);
        touchedItemList.Remove(null);
        touchedItems = touchedItemList.ToArray();
        //前の接触判定で触れたアイテムをここで一気に取得
        //(不正にアイテムを取得するチートを防止するためこのような措置を取っています)
        allowedItemEffect = true;
        for (int i = 0; i < touchedItems.Length; i++)
        {
            touchedItems[i].Activation(this);
        }
        Array.Resize(ref touchedItems, 0);
        allowedItemEffect = false;

        // 移動
        // コントローラーとキーボード両方に対応
        if (Input.GetAxis("L_Stick_H") != 0 || Input.GetAxis("L_Stick_V") != 0)
        {
            Move(GetMovement() + new Vector3(
                Input.GetAxis("L_Stick_H"),
                0,
                Input.GetAxis("L_Stick_V")).normalized
                * moveSpeed);
            direction = Mathf.Atan2(
                Input.GetAxis("L_Stick_H"), Input.GetAxis("L_Stick_V"))
                / Mathf.Deg2Rad;
            //着地していたら移動モーションを再生
            if (IsLanding())
            {
                animationName = "walk";
            }

        }

        if (Input.GetAxis("MoveLR_Horizontal") != 0 || Input.GetAxis("MoveDU_Vertical") != 0)
        {
            Move(GetMovement() + new Vector3(
                Input.GetAxis("MoveLR_Horizontal"),
                0,
                Input.GetAxis("MoveDU_Vertical")).normalized
                * moveSpeed);
            direction = Mathf.Atan2(
                Input.GetAxis("MoveLR_Horizontal"), Input.GetAxis("MoveDU_Vertical"))
                / Mathf.Deg2Rad;
            //着地していたら移動モーションを再生
            if (IsLanding())
            {
                animationName = "walk";
            }

        }

        if (IsAttacking())
        {
            //固有ワザ以外の技を使っている間は攻撃モーションを再生
            if (!IsAttacking(GetData().GetDefaultAttackMotionName()))
            {
                animationName = "attack";
            }
        }
        else if (!IsLanding())
        {
            //着地していない、かつ攻撃中でもなければ空中モーションを再生
            animationName = "midair";
        }

        //スペースキーでジャンプ
        // コントローラーならAボタン
        bool jumpInput = Input.GetKey(KeyCode.Space)
            || Input.GetKey("joystick button 0");
        //ジャンプボタンの押し始めなら
        if (jumpInput && !jumpTrigger)
        {
            Move(new Vector3(GetMovement().x, jumpPower, GetMovement().z));
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }
        jumpTrigger = jumpInput;

        attackReactionFrame = Mathf.Max(0, attackReactionFrame - 1);

        //下一段のキーを押して攻撃
        // コントローラーならBボタン
        bool attackInput = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)
            || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.V)
            || Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.N)
            || Input.GetKey(KeyCode.M)
            || Input.GetKey("joystick button 1");
        //攻撃ボタンの押し始めかつ武器を持っているなら
        if (attackInput && !attackTrigger
            && (!IsAttacking() || IsAttacking(GetData().GetDefaultAttackMotionName())))
        {
            attackReactionFrame = maxAttackReactionframe;
            if (characters.Length > 0)
            {
                //攻撃モーションを再生
                SetAttackMotion(characters[currentCharaIndex].SearchAttackMotion(
                    characters[currentCharaIndex].GetWeaponedAttackMotionName()));
                currentCharaIndex++;
            }
        }
        attackTrigger = attackInput;
        currentCharaIndex = Mathf.RoundToInt(Mathf.Repeat(currentCharaIndex,
            characters.Length));

        //自機を回転
        transform.Rotate(
            0, Input.GetAxis("R_Stick_H") * cameraControlSpeed, 0, Space.Self);
        //カメラを傾ける
        playerCameraAngle += Input.GetAxis("R_Stick_V") * cameraControlSpeed;

        transform.Rotate(
           0, Input.GetAxis("Cam_Horizontal") * cameraControlSpeed, 0, Space.Self);
        //カメラを傾ける
        playerCameraAngle += Input.GetAxis("Cam_Vertical") * cameraControlSpeed;

        //カメラの仰角値を規定範囲に収める
        playerCameraAngle = Mathf.Clamp(
            playerCameraAngle, minCameraAngle, maxCameraAngle);
        cameraAngle = playerCameraAngle;

    }

    protected override void LiveEntityOnHit(Collider col)
    {
        Item item = col.GetComponent<Item>();
        if (item)
        {
            //ここで接触したアイテムを配列に追加
            //(不正にアイテムを取得するチートを防止するためこのような措置を取っています)
            Array.Resize(ref touchedItems, touchedItems.Length + 1);
            touchedItems[touchedItems.Length - 1] = item;
        }
    }

    public void EquipCharacter(CharaData charaData)
    {
        if (allowedItemEffect)
        {
            //スロットに空きがあるなら
            if (characters.Length < maxTeamNum)
            {
                //キャラクターの配列の途中に挿入
                List<CharaData> list = new List<CharaData>(characters);
                list.Insert(Mathf.Clamp(currentCharaIndex, 0, characters.Length),
                    charaData);
                characters = list.ToArray();
            }
            else
            {
                //現在のキャラを上書き
                characters[currentCharaIndex] = charaData;
            }

            allowedItemEffect = false;
        }
    }

    public bool IsTouchedThisItem(Item item)
    {
        if (item == null)
        {
            return false;
        }
        for (int i = 0; i < touchedItems.Length; i++)
        {
            if (touchedItems[i] == item)
            {
                return true;
            }
        }
        return false;
    }
}