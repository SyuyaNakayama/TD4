using UnityEngine;
using System;
using UnityEditor;

public class Player : LiveEntity
{
    public const int maxTeamNum = 5;
    const float cameraControlSpeed = 3;
    const float moveSpeed = 1.5f;
    const float jumpPower = 10.0f;
    const float goaledCameraAngle = 0;
    const float goaledCameraDistance = 3;
    const float goaledDirection = 0;

    bool jumpTrigger;
    bool attackTrigger;
    int currentCharaIndex;
    public int GetCurrentCharaIndex()
    {
        return currentCharaIndex;
    }
    [SerializeField]
    CharaData[] characters;
    public CharaData[] GetCharacters()
    {
        return characters;
    }
    bool goaled;
    public bool GetGoaled()
    {
        return goaled;
    }
    float playerCameraAngle = maxCameraAngle;

    protected override void LiveEntityUpdate()
    {
        //生きていれば操作可能
        if (IsLive() && !GetGoaled())
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
            playerCameraAngle += Input.GetAxis("Cam_Vertical") * cameraControlSpeed;
            //カメラの仰角値を規定範囲に収める
            playerCameraAngle = Mathf.Clamp(
                playerCameraAngle, minCameraAngle, maxCameraAngle);
            cameraAngle = playerCameraAngle;
        }
        else
        {
            //カメラを演出用の位置に調整
            cameraAngle = goaledCameraAngle;
            cameraDistance = goaledCameraDistance;
            //正面を向く
            direction = goaledDirection;
            //何かボタンを押したらゴール時はステージを出る、死亡時は復活
            if (Input.GetKey(KeyCode.Space)
                || Input.GetKey("joystick button 0")
                || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)
                || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.V)
                || Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.N)
                || Input.GetKey(KeyCode.M)
                || Input.GetKey("joystick button 1"))
                if (GetGoaled())
                {
                    Quit();
                }
                else
                {
                    Revive();
                }
        }
    }

    protected override void LiveEntityOnHit(Collider col)
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
    //今いるステージの派生元として設定されているシーンに戻る
    void Quit()
    {
        foreach (StageManager obj in UnityEngine.Object.FindObjectsOfType<StageManager>())
        {
            if (obj.gameObject.activeInHierarchy)
            {
                SceneTransition.ChangeScene(obj.GetQuitSceneName());
                return;
            }
        }
    }
}