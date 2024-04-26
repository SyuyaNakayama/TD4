using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : LiveEntity
{
    static float moveSpd = 1.5f;
    static float jumpSpd = 10.0f;
    bool isLanding = false; //着地しているか
    bool jumpTrigger;

    protected override void LiveEntityUpdate()
    {
        //y軸には空気抵抗がかからないように設定
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;

        // 移動
        movement += new Vector3(
           // 右
           Convert.ToSingle(Input.GetKey(KeyCode.RightArrow)) -
           // 左
           Convert.ToSingle(Input.GetKey(KeyCode.LeftArrow)),
           0,
           // 上
           Convert.ToSingle(Input.GetKey(KeyCode.UpArrow)) -
           // 下
           Convert.ToSingle(Input.GetKey(KeyCode.DownArrow))
           ) * moveSpd;

        //スペースキーでジャンプ
        bool jumpInput = Input.GetKey(KeyCode.Space);
        //ジャンプボタンの押し始め、かつ着地しているなら
        if (jumpInput && !jumpTrigger && isLanding)
        {
            movement = new Vector3(movement.x, jumpSpd, movement.z);
        }
        jumpTrigger = jumpInput;

        //地面との接触判定を行う前に一旦着地していない状態にする
        isLanding = false;
    }
    protected override void LiveEntityCollision()
    {
        // 着地判定
        isLanding = true;
    }
}