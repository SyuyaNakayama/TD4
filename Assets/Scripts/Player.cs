using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : LiveEntity
{
    public float moveSpd = 1;
    public float jumpSpd = 10;

    protected override void LiveEntityUpdate()
    {
        // 4/22 �i�J���}
        // �ړ����x���v�Z�i�C�ɐH��Ȃ��Ȃ�����č\��Ȃ��j
        movement.x +=
            // �E
            (Convert.ToSingle(Input.GetKey(KeyCode.RightArrow)) -
            // ��
            Convert.ToSingle(Input.GetKey(KeyCode.LeftArrow))) * moveSpd;
        movement.z +=
            // ��
            (Convert.ToSingle(Input.GetKey(KeyCode.UpArrow)) -
            // ��
            Convert.ToSingle(Input.GetKey(KeyCode.DownArrow))) * moveSpd;

        //���[�L�[�ŏ㉺���E�ɉ���
        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    movement += new Vector3(0, 0, moveSpd);
        //}
        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    movement += new Vector3(0, 0, -moveSpd);
        //}
        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    movement += new Vector3(moveSpd, 0, 0);
        //}
        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    movement += new Vector3(-moveSpd, 0, 0);
        //}

        //�X�y�[�X�L�[�ŃW�����v
        if (Input.GetKey(KeyCode.Space))
        {
            movement = new Vector3(movement.x, jumpSpd, movement.z);
        }
    }
}