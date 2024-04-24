using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : LiveEntity
{
    static float moveSpd = 1.5f;
    static float jumpSpd = 10.0f;

    bool jumpTrigger;

    protected override void LiveEntityUpdate()
    {
        //y軸には空気抵抗がかからないように設定
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;

        // 4/24 テラオ
        // 上のやつを最適化
        // 4/24 ナカヤマ　サンクス
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
        if (jumpInput && !jumpTrigger)
        {
            movement = new Vector3(movement.x, jumpSpd, movement.z);
        }
        jumpTrigger = jumpInput;
    }
}