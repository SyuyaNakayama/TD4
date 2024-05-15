using UnityEngine;
using System;
using UnityEditor;

public class Player : LiveEntity
{
    static float moveSpeed = 1.5f;
    static float jumpPower = 10.0f;
    static int maxTeamNum = 5;

    bool jumpTrigger;


    protected override void LiveEntityUpdate()
    {
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
        //ジャンプボタンの押し始めなら
        if (jumpInput && !jumpTrigger)
        {
            movement = new Vector3(movement.x, jumpPower, movement.z);
        }
        jumpTrigger = jumpInput;
    }
}