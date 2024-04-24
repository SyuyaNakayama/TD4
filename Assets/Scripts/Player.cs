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
        //y軸には空気抵抗がかからないように設定
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;

        // 4/22 ナカヤマ
        // 移動速度を計算（気に食わないなら消して構わない）
        //movement.x +=
        //    // 右
        //    (Convert.ToSingle(Input.GetKey(KeyCode.RightArrow)) -
        //    // 左
        //    Convert.ToSingle(Input.GetKey(KeyCode.LeftArrow))) * moveSpd;
        //movement.z +=
        //    // 上
        //    (Convert.ToSingle(Input.GetKey(KeyCode.UpArrow)) -
        //    // 下
        //    Convert.ToSingle(Input.GetKey(KeyCode.DownArrow))) * moveSpd;

        // 4/24 テラオ
        // 上のやつを最適化
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