using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

public class Player : LiveEntity
{
    public const int maxTeamNum = 5;
    const float cameraControlSpeed = 3;
    const float moveSpeed = 1.5f;
    const float jumpPower = 10.0f;

    bool jumpTrigger;
    bool attackTrigger;
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
        //�L�����N�^�[�̔z�񂩂�null������
        List<CharaData> characterList = new List<CharaData>(characters);
        characterList.Remove(null);
        characters = characterList.ToArray();

        //�ڐG�����A�C�e���̔z�񂩂�null������
        List<Item> touchedItemList = new List<Item>(touchedItems);
        touchedItemList.Remove(null);
        touchedItems = touchedItemList.ToArray();
        //�O�̐ڐG����ŐG�ꂽ�A�C�e���������ň�C�Ɏ擾
        //(�s���ɃA�C�e�����擾����`�[�g��h�~���邽�߂��̂悤�ȑ[�u������Ă��܂�)
        allowedItemEffect = true;
        for (int i = 0; i < touchedItems.Length; i++)
        {
            touchedItems[i].Activation(this);
        }
        Array.Resize(ref touchedItems, 0);
        allowedItemEffect = false;

        // �ړ�
        // �R���g���[���[�ƃL�[�{�[�h�����ɑΉ�
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Move(GetMovement() + new Vector3(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")).normalized
                * moveSpeed);
            direction = Mathf.Atan2(
                Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))
                / Mathf.Deg2Rad;
        }

        //�X�y�[�X�L�[�ŃW�����v
        // �R���g���[���[�Ȃ�A�{�^��
        bool jumpInput = Input.GetKey(KeyCode.Space)
            || Input.GetKey("joystick button 0");
        //�W�����v�{�^���̉����n�߂Ȃ�
        if (jumpInput && !jumpTrigger)
        {
            Move(new Vector3(GetMovement().x, jumpPower, GetMovement().z));
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }
        jumpTrigger = jumpInput;

        //����i�̃L�[�������čU��
        // �R���g���[���[�Ȃ�B�{�^��
        bool attackInput = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)
            || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.V)
            || Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.N)
            || Input.GetKey(KeyCode.M)
            || Input.GetKey("joystick button 1");
        //�U���{�^���̉����n�߂�����������Ă���Ȃ�
        if (attackInput && !attackTrigger && !IsAttacking()
            && characters.Length > 0)
        {
            //�U�����[�V�������Đ�
            SetAttackMotion(characters[currentCharaIndex].SearchAttackMotion(
                characters[currentCharaIndex].GetWeaponedAttackMotionName()));
            currentCharaIndex++;
        }
        attackTrigger = attackInput;
        currentCharaIndex = Mathf.RoundToInt(Mathf.Repeat(currentCharaIndex,
            characters.Length));

        //���@����]
        transform.Rotate(
            0, Input.GetAxis("Cam_Horizontal") * cameraControlSpeed, 0, Space.Self);
        //�J�������X����
        playerCameraAngle += Input.GetAxis("Cam_Vertical") * cameraControlSpeed;
        //�J�����̋p�l���K��͈͂Ɏ��߂�
        playerCameraAngle = Mathf.Clamp(
            playerCameraAngle, minCameraAngle, maxCameraAngle);
        cameraAngle = playerCameraAngle;
    }

    protected override void LiveEntityOnHit(Collider col)
    {
        Item item = col.GetComponent<Item>();
        if (item != null)
        {
            //�����ŐڐG�����A�C�e����z��ɒǉ�
            //(�s���ɃA�C�e�����擾����`�[�g��h�~���邽�߂��̂悤�ȑ[�u������Ă��܂�)
            Array.Resize(ref touchedItems, touchedItems.Length + 1);
            touchedItems[touchedItems.Length - 1] = item;
        }
    }

    public void EquipCharacter(CharaData charaData)
    {
        if (allowedItemEffect)
        {
            //�X���b�g�ɋ󂫂�����Ȃ�
            if (characters.Length < maxTeamNum)
            {
                //�L�����N�^�[�̔z��̓r���ɑ}��
                List<CharaData> list = new List<CharaData>(characters);
                list.Insert(Mathf.Clamp(currentCharaIndex, 0, characters.Length),
                    charaData);
                characters = list.ToArray();
            }
            else
            {
                //���݂̃L�������㏑��
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