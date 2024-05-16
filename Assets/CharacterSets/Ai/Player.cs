using UnityEngine;
using System;
using UnityEditor;

public class Player : LiveEntity
{
    static float moveSpeed = 1.5f;
    static float jumpPower = 10.0f;
    static int maxTeamNum = 5;

    bool jumpTrigger;
    bool attackTrigger;
    int currentCharaIndex;
    [SerializeField]
    CharaData characters;

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

        //下一段のキーを押して攻撃
        // コントローラーならBボタン
        bool attackInput = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)
            || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.V)
            || Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.N)
            || Input.GetKey(KeyCode.M)
            || Input.GetKey("joystick button 1");
        //攻撃ボタンの押し始めなら
        if (attackInput && !attackTrigger)
        {
            //攻撃モーションを再生
            SetAttackMotion(characters.SearchAttackMotion(
                characters.GetWeaponedAttackMotionName()));
        }
        attackTrigger = attackInput;
    }
}