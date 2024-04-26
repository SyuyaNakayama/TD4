using UnityEngine;
using System;

public class Player : LiveEntity
{
    static float moveSpeed = 1.5f;
    static float jumpPower = 10.0f;

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
           ) * moveSpeed;

        //スペースキーでジャンプ
        bool jumpInput = Input.GetKey(KeyCode.Space);
        //ジャンプボタンの押し始め、かつ着地しているなら
        if (jumpInput && !jumpTrigger && GetIsLanding())
        {
            movement = new Vector3(movement.x, jumpPower, movement.z);
        }
        jumpTrigger = jumpInput;
    }
}