using UnityEngine;
using System;
using UnityEditor;

public class Player : LiveEntity
{
    const float cameraControlSpeed = 3;

    static float moveSpeed = 1.5f;
    static float jumpPower = 10.0f;
    static int maxTeamNum = 5;
    static float goaledCameraAngle = 0;
    static float goaledDirection = 0;
    bool jumpTrigger;
    bool attackTrigger;
    int currentCharaIndex;
    [SerializeField]
    CharaData[] characters;
    bool goaled;
    public bool GetGoaled()
    {
        return goaled;
    }

    protected override void LiveEntityUpdate()
    {
        //生きていれば操作可能
        if (IsLive())
        {
            // 移動
            // コントローラーとキーボード両方に対応
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                Move(GetMovement() + new Vector3(
                    Input.GetAxis("Horizontal"),
                    0,
                    Input.GetAxis("Vertical")).normalized
                    * moveSpeed);
                direction = Mathf.Atan2(
                    Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))
                     / Mathf.Deg2Rad;
            }

            //スペースキーでジャンプ
            // コントローラーならAボタン
            bool jumpInput = Input.GetKey(KeyCode.Space)
                || Input.GetKey("joystick button 0");
            //ジャンプボタンの押し始めなら
            if (jumpInput && !jumpTrigger)
            {
                Move(new Vector3(GetMovement().x, jumpPower, GetMovement().z));
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
            if (attackInput && !attackTrigger && !IsAttacking())
            {
                //攻撃モーションを再生
                SetAttackMotion(characters[currentCharaIndex].SearchAttackMotion(
                    characters[currentCharaIndex].GetWeaponedAttackMotionName()));
                currentCharaIndex++;
            }
            attackTrigger = attackInput;

            currentCharaIndex = Mathf.RoundToInt(Mathf.Repeat(currentCharaIndex,
                characters.Length));

            //自機を回転
            transform.Rotate(
                0, Input.GetAxis("Cam_Horizontal") * cameraControlSpeed, 0, Space.Self);
            //カメラを傾ける
            cameraAngle += Input.GetAxis("Cam_Vertical") * cameraControlSpeed;
        }
    }

    protected virtual void LiveEntityOnHit(Collider col)
    {
        if (col.GetComponent<Goal>() != null)
        {
            Clear();
        }
    }

    //ゴールに入った時の処理
    void Clear()
    {
        goaled = true;
    }
    //攻撃状態の取得
    public bool GetAttackTrigger()
    {
        return attackTrigger;
    }
}