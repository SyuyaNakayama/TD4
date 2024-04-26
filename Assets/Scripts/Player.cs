using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : LiveEntity
{
    static float moveSpd = 1.5f;
    static float jumpSpd = 10.0f;
    bool isJump = false; // �W�����v����
    bool isJumpMoment = false; // �W�����v�����u�Ԃ�
    bool jumpTrigger;

    protected override void LiveEntityUpdate()
    {
        isJumpMoment = false;
        //y���ɂ͋�C��R��������Ȃ��悤�ɐݒ�
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;

        // �ړ�
        movement += new Vector3(
           // �E
           Convert.ToSingle(Input.GetKey(KeyCode.RightArrow)) -
           // ��
           Convert.ToSingle(Input.GetKey(KeyCode.LeftArrow)),
           0,
           // ��
           Convert.ToSingle(Input.GetKey(KeyCode.UpArrow)) -
           // ��
           Convert.ToSingle(Input.GetKey(KeyCode.DownArrow))
           ) * moveSpd;

        //�X�y�[�X�L�[�ŃW�����v
        bool jumpInput = Input.GetKey(KeyCode.Space);
        if (jumpInput && !jumpTrigger&& !isJump)
        {
            movement = new Vector3(movement.x, jumpSpd, movement.z);
            isJump = isJumpMoment = true;
        }
        jumpTrigger = jumpInput;
    }
    protected override void LiveEntityCollision()
    {
        // ���n����
        if (!isJumpMoment) { isJump = false; }
    }
}