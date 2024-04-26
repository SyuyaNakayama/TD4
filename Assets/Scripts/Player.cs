using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : LiveEntity
{
    static float moveSpd = 1.5f;
    static float jumpSpd = 10.0f;
    bool isJump = false; // ジャンプ中か
    bool isJumpMoment = false; // ジャンプした瞬間か
    bool jumpTrigger;

    protected override void LiveEntityUpdate()
    {
        isJumpMoment = false;
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
        if (jumpInput && !jumpTrigger&& !isJump)
        {
            movement = new Vector3(movement.x, jumpSpd, movement.z);
            isJump = isJumpMoment = true;
        }
        jumpTrigger = jumpInput;
    }
    protected override void LiveEntityCollision()
    {
        // 着地判定
        if (!isJumpMoment) { isJump = false; }
    }
}