using UnityEngine;
using System;
using UnityEditor;

public class Player : LiveEntity
{
    const float cameraControlSpeed = 3;

    static float moveSpeed = 1.5f;
    static float jumpPower = 10.0f;
    static int maxTeamNum = 5;
    bool jumpTrigger;
    bool attackTrigger;
    int currentCharaIndex;

    [SerializeField]
    CharaData[] characters;

    protected override void LiveEntityUpdate()
    {
        // �ړ�
        // �R���g���[���[�ƃL�[�{�[�h�����ɑΉ�
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            movement += new Vector3(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")).normalized
                * moveSpeed;
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
            movement = new Vector3(movement.x, jumpPower, movement.z);
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
        cameraAngle += Input.GetAxis("Cam_Vertical") * cameraControlSpeed;
    }
}