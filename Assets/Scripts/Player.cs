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
        // 4/22 ナカヤマ
        // 移動速度を計算（気に食わないなら消して構わない）
        movement.x +=
            // 右
            (Convert.ToSingle(Input.GetKey(KeyCode.RightArrow)) -
            // 左
            Convert.ToSingle(Input.GetKey(KeyCode.LeftArrow))) * moveSpd;
        movement.z +=
            // 上
            (Convert.ToSingle(Input.GetKey(KeyCode.UpArrow)) -
            // 下
            Convert.ToSingle(Input.GetKey(KeyCode.DownArrow))) * moveSpd;

        //矢印ーキーで上下左右に加速
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

        //スペースキーでジャンプ
        if (Input.GetKey(KeyCode.Space))
        {
            movement = new Vector3(movement.x, jumpSpd, movement.z);
        }
    }
}