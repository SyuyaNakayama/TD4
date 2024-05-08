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
        //y軸には空気抵抗がかからないように設定
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;

        // 移動
        // コントローラーとキーボード両方に対応
        movement += new Vector3(
        Input.GetAxis("Horizontal"),
        0,
        Input.GetAxis("Vertical")).normalized
        * moveSpeed;

        //スペースキーでジャンプ
        // コントローラーならAボタン
        bool jumpInput = Input.GetKey(KeyCode.Space)
            || Input.GetKey("joystick button 0");
        //ジャンプボタンの押し始め、かつ着地しているなら
        if (jumpInput && !jumpTrigger && GetIsLanding())
        {
            movement = new Vector3(movement.x, jumpPower, movement.z);
        }
        jumpTrigger = jumpInput;
    }
}