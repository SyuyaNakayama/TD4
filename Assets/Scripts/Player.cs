using UnityEngine;
using System;
using UnityEditor;

public class Player : LiveEntity
{
    static float moveSpeed = 1.5f;
    static float jumpPower = 10.0f;

    bool jumpTrigger;

    private void Start()
    {
        maxHP = 50;
    }

    protected override void LiveEntityUpdate()
    {
        //y���ɂ͋�C��R��������Ȃ��悤�ɐݒ�
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;

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
        //�W�����v�{�^���̉����n�߁A�����n���Ă���Ȃ�
        if (jumpInput && !jumpTrigger && GetIsLanding())
        {
            movement = new Vector3(movement.x, jumpPower, movement.z);
        }
        jumpTrigger = jumpInput;
    }
}