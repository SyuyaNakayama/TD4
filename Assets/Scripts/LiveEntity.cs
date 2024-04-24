using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveEntity : MonoBehaviour
{
    public struct AxisSwitch
    {
        public bool x;
        public bool y;
        public bool z;
    }

    public float drag = 0.8f;
    protected Vector3 movement;
    protected AxisSwitch dragAxis;
    Vector3 prevPos;
    Quaternion prevRot;

    void Awake()
    {
        prevPos = transform.position;
        prevRot = transform.rotation;
    }

    //物理演算が更新されるタイミングで毎フレーム呼ばれる
    //注意！　Update()とは呼ばれる周期が異なるため周期ズレによる不具合に気を付けて下さい
    void FixedUpdate()
    {
        //前フレームからの移動量をmovementに変換
        movement = Quaternion.Inverse(prevRot) * ((transform.position - prevPos) / Time.deltaTime);
        prevPos = transform.position;
        prevRot = transform.rotation;

        //重力及び空気抵抗
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
        movement += new Vector3(0, -0.5f, 0);

        //ここで各派生クラスの固有更新処理を呼ぶ
        LiveEntityUpdate();

        //movementをvelocityに変換
        GetComponent<Rigidbody>().velocity = transform.rotation * movement;
    }

    //このオブジェクトがコライダーに触れている間毎フレームこの関数が呼ばれる（触れているコライダーが自動的に引数に入る）
    void OnCollisionStay(Collision col)
    {
        //足を向けるべき位置を算出し、
        Vector3 localClosestPoint = transform.InverseTransformPoint(
            col.collider.ClosestPoint(transform.position));
        //x軸を中心にその位置を向くように回転させる
        transform.Rotate(
            -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y) / Mathf.Deg2Rad, 0, 0, Space.Self);

        //再度足を向けるべき位置を算出し、
        localClosestPoint = transform.InverseTransformPoint(
            col.collider.ClosestPoint(transform.position));
        //z軸を中心にその位置を向くように回転させる
        transform.Rotate(0, 0, 
            Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y) / Mathf.Deg2Rad, Space.Self);

        //ここで各派生クラスの固有コライダー処理を呼ぶ
        LiveEntityCollision();
    }

    //各派生クラスの固有更新処理（派生クラス内でオーバーライドして使う）
    protected virtual void LiveEntityUpdate()
    {

    }

    //各派生クラスの固有コライダー処理（派生クラス内でオーバーライドして使う）
    protected virtual void LiveEntityCollision()
    {

    }
}
