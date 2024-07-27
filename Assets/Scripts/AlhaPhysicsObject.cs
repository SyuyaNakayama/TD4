using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class AlhaPhysicsObject : MonoBehaviour
{
    Vector3 prevPos;
    public Vector3 GetPrevPos()
    {
        return prevPos;
    }
    protected Vector3 movement;
    public Vector3 GetMovement()
    {
        return movement;
    }
    Vector3 preMovement;
    public Vector3 localGrandMove
    {
        get;
        private set;
    }
    public Vector3 fieldMove
    {
        get;
        private set;
    }
    public Vector3 pushBackMove
    {
        get;
        private set;
    }
    bool isLanding = false; //着地しているか
    public bool GetIsLanding()
    {
        return isLanding;
    }
    [SerializeField]
    protected float drag = 0.8f;
    [SerializeField]
    protected KX_netUtil.AxisSwitch dragAxis;
    [SerializeField]
    protected float gravityScale;
    [SerializeField]
    bool noGravity;

    void Awake()
    {
        prevPos = transform.position;

        APOAwake();
    }

    //物理演算が更新されるタイミングで毎フレーム呼ばれる
    //注意！　Update()とは呼ばれる周期が異なるため周期ズレによる不具合に気を付けて下さい
    void FixedUpdate()
    {
        APOUpdate();

        //【重要】ここから「preMovement = movement;」までmovementの値を書き換えないこと
        //movementをvelocityに変換
        GetComponent<Rigidbody>().velocity =
            transform.rotation * movement * transform.localScale.x;

        Vector3 playerLocalPosPin = transform.InverseTransformPoint(prevPos);
        prevPos = transform.position;

        //ギミックによる移動に関する更新処理
        GetComponent<Rigidbody>().velocity += fieldMove;
        playerLocalPosPin += transform.InverseTransformPoint(transform.position + fieldMove * Time.deltaTime);
        localGrandMove = -playerLocalPosPin;
        fieldMove = Vector3.zero;

        GetComponent<Rigidbody>().velocity += pushBackMove;
        pushBackMove = Vector3.zero;

        Vector3 movementDiff = movement - preMovement;
        preMovement = movement;
        //これ以降はmovementの値を書き換えて良い

        //着地判定
        Vector3 pushBackedMovement =
            localGrandMove / Time.deltaTime + movementDiff;

        if (Vector3.Magnitude(pushBackedMovement) < Vector3.Magnitude(movement))
        {
            movement = Vector3.Lerp(movement, pushBackedMovement, 0.5f);
        }

        //空気抵抗
        if (dragAxis.x && dragAxis.y && dragAxis.z)
        {
            movement *= drag;
        }
        else
        {
            if (dragAxis.x)
            {
                movement.x *= drag;
            }
            if (dragAxis.y)
            {
                movement.y *= drag;
            }
            if (dragAxis.z)
            {
                movement.z *= drag;
            }
        }
        //重力
        if (!noGravity)
        {
            movement += new Vector3(0, -gravityScale, 0);
        }
        noGravity = false;

        //地面との接触判定を行う前に一旦着地していない状態にする
        isLanding = false;
    }

    //リフトに乗っている時や風に煽られているの動きを実現するための関数
    public void AddFieldMove(Vector3 force)
    {
        fieldMove += force;
    }
    //擬似的に壁に押されたような動きを実現するための関数
    public void AddPushBackMove(Vector3 force)
    {
        pushBackMove += force;
    }
    //このフレームのみ重力の影響を受けなくする
    public void SetNoGravity()
    {
        noGravity = true;
    }

    protected virtual void APOUpdate()
    {
    }
    protected virtual void APOAwake()
    {
    }
}