using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : LiveEntity
{
    static float moveSpd = 1.5f;
    static float jumpSpd = 10.0f;
    bool isLanding = false; //���n���Ă��邩
    bool jumpTrigger;

    protected override void LiveEntityUpdate()
    {
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
        //�W�����v�{�^���̉����n�߁A�����n���Ă���Ȃ�
        if (jumpInput && !jumpTrigger && isLanding)
        {
            movement = new Vector3(movement.x, jumpSpd, movement.z);
        }
        jumpTrigger = jumpInput;

        //�n�ʂƂ̐ڐG������s���O�Ɉ�U���n���Ă��Ȃ���Ԃɂ���
        isLanding = false;
    }
    protected override void LiveEntityCollision()
    {
        // ���n����
        isLanding = true;
    }
}