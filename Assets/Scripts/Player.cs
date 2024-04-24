using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : LiveEntity
{
    static float moveSpd = 1.5f;
    static float jumpSpd = 10;

    bool jumpTrigger;

    protected override void LiveEntityUpdate()
    {
        //y���ɂ͋�C��R��������Ȃ��悤�ɐݒ�
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;

        // 4/22 �i�J���}
        // �ړ����x���v�Z�i�C�ɐH��Ȃ��Ȃ�����č\��Ȃ��j
        //movement.x +=
        //    // �E
        //    (Convert.ToSingle(Input.GetKey(KeyCode.RightArrow)) -
        //    // ��
        //    Convert.ToSingle(Input.GetKey(KeyCode.LeftArrow))) * moveSpd;
        //movement.z +=
        //    // ��
        //    (Convert.ToSingle(Input.GetKey(KeyCode.UpArrow)) -
        //    // ��
        //    Convert.ToSingle(Input.GetKey(KeyCode.DownArrow))) * moveSpd;

        // 4/24 �e���I
        // ��̂���œK��
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
        if (jumpInput && !jumpTrigger)
        {
            movement = new Vector3(movement.x, jumpSpd, movement.z);
        }
        jumpTrigger = jumpInput;
    }
}