using UnityEngine;
using System;

public class Player : LiveEntity
{
    static float moveSpeed = 1.5f;
    static float jumpPower = 10.0f;

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
           ) * moveSpeed;

        //�X�y�[�X�L�[�ŃW�����v
        bool jumpInput = Input.GetKey(KeyCode.Space);
        //�W�����v�{�^���̉����n�߁A�����n���Ă���Ȃ�
        if (jumpInput && !jumpTrigger && GetIsLanding())
        {
            movement = new Vector3(movement.x, jumpPower, movement.z);
        }
        jumpTrigger = jumpInput;
    }
}