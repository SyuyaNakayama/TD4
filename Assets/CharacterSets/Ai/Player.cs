using UnityEngine;
using System;
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

    protected override void LiveEntityUpdate()
    {
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
        //�U���{�^���̉����n�߂Ȃ�
        if (attackInput && !attackTrigger && !IsAttacking())
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
}