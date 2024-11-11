using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class AlhaPhysicsObject : MonoBehaviour
{
    Vector3 prevPos;
    Vector3 move;
    Vector3 prevMove;
    protected Vector3 movement;
    public Vector3 GetMovement()
    {
        return movement;
    }
    Vector3 prevMovement;
    public Vector3 GetPrevMovement()
    {
        return prevMovement;
    }
    Vector3 pushBackedMovement;
    public Vector3 GetPushBackedMovement()
    {
        return pushBackedMovement;
    }
    [SerializeField]
    protected bool isAllowMove;
    Vector3 forceTranslate;
    public Vector3 GetForceTranslate()
    {
        return forceTranslate;
    }
    Vector3 prevForceTranslate;
    public Vector3 GetPrevForceTranslate()
    {
        return prevForceTranslate;
    }
    Vector3 pushBackVec;
    public Vector3 GetPushBackVec()
    {
        return pushBackVec;
    }

    protected Vector3 fieldMove;
    protected Vector3 pushBackMove;
    protected int landing;
    [SerializeField]
    protected float drag = 0.8f;
    [SerializeField]
    protected KX_netUtil.AxisSwitch dragAxis;
    [SerializeField]
    protected Vector3 gravityVec;
    [SerializeField]
    protected bool noGravity;

    void Awake()
    {
        prevPos = transform.position;

        APOAwake();
    }

    //物理演算が更新されるタイミングで毎フレーム呼ばれる
    //注意！　Update()とは呼ばれる周期が異なるため周期ズレによる不具合に気を付けて下さい
    void FixedUpdate()
    {
        //空気抵抗
        float velocityDiffuse = Mathf.Max(drag, 0) + 1;
        if (dragAxis.x && dragAxis.y && dragAxis.z)
        {
            movement /= velocityDiffuse;
        }
        else
        {
            if (dragAxis.x)
            {
                movement.x /= velocityDiffuse;
            }
            if (dragAxis.y)
            {
                movement.y /= velocityDiffuse;
            }
            if (dragAxis.z)
            {
                movement.z /= velocityDiffuse;
            }
        }
        //重力
        if (!noGravity)
        {
            movement += gravityVec;
        }
        noGravity = false;

        Vector3 playerLocalPosPin = transform.InverseTransformPoint(prevPos);
        playerLocalPosPin += transform.InverseTransformPoint(
            transform.position + fieldMove * Time.deltaTime);
        prevMove = move;
        move = transform.position - prevPos;
        prevPos = transform.position;

        //壁などに押し返されたらmovementを減衰
        Vector3 prevPushBackedMovement = pushBackedMovement;
        pushBackedMovement =
            -playerLocalPosPin / Time.deltaTime;
        pushBackedMovement =
            MinimizeMovement(prevMovement, pushBackedMovement);

        //movement以外によって生じた移動をforceTranslateに格納
        prevForceTranslate = forceTranslate;
        forceTranslate = prevMove
            - (transform.rotation * prevPushBackedMovement)
            * transform.localScale.x * Time.deltaTime;

        Vector3 holdPos = transform.position;
        APOUpdate();
        transform.position = holdPos;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        //movementをvelocityに変換
        rigidbody.velocity =
            transform.rotation * movement * transform.localScale.x;
        Vector3 movementDiff = movement - prevMovement;
        prevMovement = movement;

        Vector3 addedPushBackedMovement =
        pushBackedMovement + movementDiff;

        addedPushBackedMovement =
            MinimizeMovement(movement, addedPushBackedMovement);

        pushBackVec = addedPushBackedMovement - movement;

        //着地判定
        landing = Mathf.Max(0, landing - 1);
        if (Vector3.Dot(-gravityVec, pushBackVec) > 0.01f)
        {
            landing = 1;
        }

        movement = addedPushBackedMovement;

        //ギミックによる移動に関する更新処理
        //空間ごと動いているものとみなして移動
        rigidbody.velocity += fieldMove;
        fieldMove = Vector3.zero;
        //壁に押されたものとみなして移動
        rigidbody.velocity += pushBackMove;
        pushBackMove = Vector3.zero;
    }

    //動ける状態なら移動
    public void SetMovement(Vector3 setMovement)
    {
        if (isAllowMove)
        {
            movement = setMovement;
        }
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
    public void SetNoGravity(bool setNoGravity)
    {
        noGravity = setNoGravity;
    }
    //着地しているか
    public bool IsLanding()
    {
        return landing > 0;
    }

    //
    Vector3 MinimizeMovement(Vector3 setMovement, Vector3 setPushBackedMovement)
    {
        if (Mathf.Sign(setMovement.x) != Mathf.Sign(setPushBackedMovement.x))
        {
            setPushBackedMovement =
                new Vector3(0, setPushBackedMovement.y, setPushBackedMovement.z);
        }
        else if (Mathf.Abs(setMovement.x) < Mathf.Abs(setPushBackedMovement.x))
        {
            setPushBackedMovement =
                new Vector3(setMovement.x, setPushBackedMovement.y, setPushBackedMovement.z);
        }

        if (Mathf.Sign(setMovement.y) != Mathf.Sign(setPushBackedMovement.y))
        {
            setPushBackedMovement =
                new Vector3(setPushBackedMovement.x, 0, setPushBackedMovement.z);
        }
        else if (Mathf.Abs(setMovement.y) < Mathf.Abs(setPushBackedMovement.y))
        {
            setPushBackedMovement =
                new Vector3(setPushBackedMovement.x, setMovement.y, setPushBackedMovement.z);
        }

        if (Mathf.Sign(setMovement.z) != Mathf.Sign(setPushBackedMovement.z))
        {
            setPushBackedMovement =
                new Vector3(setPushBackedMovement.x, setPushBackedMovement.y, 0);
        }
        else if (Mathf.Abs(setMovement.z) < Mathf.Abs(setPushBackedMovement.z))
        {
            setPushBackedMovement =
                new Vector3(setPushBackedMovement.x, setPushBackedMovement.y, setMovement.z);
        }

        return setPushBackedMovement;
    }

    protected virtual void APOUpdate()
    {
    }
    protected virtual void APOAwake()
    {
    }
}