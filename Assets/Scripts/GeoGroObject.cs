using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class GeoGroObject : UnLandableObject
{
    [SerializeField]
    Collider currentGround;
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
    protected bool allowGroundSet = true;
    [SerializeField]
    protected float drag = 0.8f;
    [SerializeField]
    protected KX_netUtil.AxisSwitch dragAxis;
    [SerializeField]
    protected float gravityScale;


    void Awake()
    {
        prevPos = transform.position;

        GGOAwake();
    }

    //物理演算が更新されるタイミングで毎フレーム呼ばれる
    //注意！　Update()とは呼ばれる周期が異なるため周期ズレによる不具合に気を付けて下さい
    void FixedUpdate()
    {
        //足を地面に向ける
        if (currentGround != null
            && currentGround.ClosestPoint(transform.position) != transform.position)
        {
            //足を向けるべき位置を算出する
            Vector3 localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //どちらかといえば縦向きに大きく回転する必要があるなら
            if (Mathf.Abs(localClosestPoint.z) > Mathf.Abs(localClosestPoint.x))
            {
                //x軸を中心にその位置を向くように回転させる
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);

                //再度足を向けるべき位置を算出し、
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //z軸を中心にその位置を向くように回転させる
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);
            }
            else
            {
                //z軸を中心にその位置を向くように回転させる
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);

                //再度足を向けるべき位置を算出し、
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //x軸を中心にその位置を向くように回転させる
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);
            }
        }

        GGOUpdate();

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
        movement += new Vector3(0, -gravityScale, 0);

        //地面との接触判定を行う前に一旦着地していない状態にする
        isLanding = false;
    }

    //このオブジェクトがコライダーに触れている間毎フレームこの関数が呼ばれる（触れているコライダーが自動的に引数に入る）
    //注意！　OnTriggerStay()と違って剛体同士の衝突判定専用です
    void OnCollisionStay(Collision col)
    {
        if (col.collider.gameObject.GetComponent<UnLandableObject>() == null && allowGroundSet)
        {
            //足を向けるべき地形として登録
            currentGround = col.collider;
            // 着地判定
            isLanding = true;
        }

        GGOOnCollisionStay(col);
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

    protected virtual void GGOUpdate()
    {
    }
    protected virtual void GGOAwake()
    {
    }
    protected virtual void GGOOnCollisionStay(Collision col)
    {
    }
}