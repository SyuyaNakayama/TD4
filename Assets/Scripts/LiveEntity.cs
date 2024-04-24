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

    protected Vector3 movement;
    protected AxisSwitch dragAxis;
    protected 

    void Start()
    {
        
    }

    //物理演算が更新されるタイミングで毎フレーム呼ばれる
    //注意！　Update()とは呼ばれる周期が異なるため周期ずれによる不具合に気を付けて下さい
    void FixedUpdate()
    {
        //ここで各派生クラスの固有更新処理を呼ぶ
        LiveEntityUpdate();

        //ここで
        GetComponent<Rigidbody>().velocity = transform.rotation * movement;

        //重力及び空気抵抗

        if (dragAxis.x && dragAxis.y && dragAxis.z)
        {
            movement *= 0.8f;
        }
        else
        {
            if(dragAxis.x)
            {
                movement.x *= 0.8f;
            }
            if (dragAxis.y)
            {
                movement.y *= 0.8f;
            }
            if (dragAxis.z)
            {
                movement.z *= 0.8f;
            }
        }
        movement += new Vector3(0, -0.5f, 0);
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
    }

    //各派生クラスの固有更新処理（派生クラス内でオーバーライドして使う）
    protected virtual void LiveEntityUpdate()
    {

    }
}
