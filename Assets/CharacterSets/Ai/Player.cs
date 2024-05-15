using UnityEngine;
using System;
using UnityEditor;

public class Player : LiveEntity
{
    static float moveSpeed = 1.5f;
    static float jumpPower = 10.0f;
    static int maxTeamNum = 5;

    bool jumpTrigger;
    bool attackTrigger;
    int currentCharaIndex;
    [SerializeField]
    CharaData characters;

    protected override void LiveEntityUpdate()
    {
        // �ړ�
        // �R���g���[���[�ƃL�[�{�[�h�����ɑΉ�
        movement += new Vector3(
        Input.GetAxis("Horizontal"),
        0,
        Input.GetAxis("Vertical")).normalized
        * moveSpeed;

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
        if (attackInput && !attackTrigger)
        {
            //�U�����[�V�������Đ�
            SetAttackMotion(characters.SearchAttackMotion(
                characters.GetWeaponedAttackMotionName()));
        }
        attackTrigger = attackInput;
    }
}