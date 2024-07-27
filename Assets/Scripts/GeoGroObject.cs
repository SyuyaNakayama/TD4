using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class GeoGroObject : AlhaPhysicsObject
{
    [SerializeField]
    Collider currentGround;
    Collider[] touchedGrounds = { };
    [SerializeField]
    protected bool allowGroundSet = true;

    protected override void APOAwake()
    {
        GGOAwake();
    }

    //物理演算が更新されるタイミングで毎フレーム呼ばれる
    //注意！　Update()とは呼ばれる周期が異なるため周期ズレによる不具合に気を付けて下さい
    protected override void APOUpdate()
    {
        Collider tempGround = currentGround;
        //触れたコライダーのうち最も近いものを一旦自身の足場とする
        float nearestGroundDistance = 0;
        bool detected = false;
        for (int i = 0; i < touchedGrounds.Length; i++)
        {
            float currentGroundDistance = Vector3.Magnitude(
                touchedGrounds[i].ClosestPoint(transform.position) - transform.position);
            if ((!detected || currentGroundDistance < nearestGroundDistance))
            {
                tempGround = touchedGrounds[i];
                nearestGroundDistance = currentGroundDistance;
                detected = true;
            }
        }

        //そのコライダーが他のコライダーとクラスターになっているか検索
        foreach (MargedGround obj in UnityEngine.Object.FindObjectsOfType<MargedGround>())
        {
            MargedGround.GroundCluster[] groundClusters = obj.GetGroundClusters();
            for (int i = 0; i < groundClusters.Length; i++)
            {
                Collider[] currentColliders = groundClusters[i].colliders;
                for (int j = 0; j < currentColliders.Length; j++)
                {
                    //クラスターが見つかったらその中で最も近いものを一旦自身の足場とする
                    if (currentColliders[j] == tempGround)
                    {
                        detected = false;
                        for (int k = 0; k < currentColliders.Length; k++)
                        {
                            if (currentColliders[k] != null)
                            {
                                float currentGroundDistance = Vector3.Magnitude(
                                    currentColliders[k].ClosestPoint(transform.position) - transform.position);
                                if ((!detected || currentGroundDistance < nearestGroundDistance)
                                    && currentColliders[k].GetComponent<UnLandableObject>() == null)
                                {
                                    tempGround = currentColliders[k];
                                    nearestGroundDistance = currentGroundDistance;
                                    detected = true;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        //そのコライダーが着地できるものであれば正式に自身の足場とする
        if (tempGround != null && tempGround.GetComponent<UnLandableObject>() == null)
        {
            currentGround = tempGround;
        }

        //触れたコライダーの情報をリセット
        Array.Resize(ref touchedGrounds, 0);

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
    }

    //このオブジェクトがコライダーに触れている間毎フレームこの関数が呼ばれる（触れているコライダーが自動的に引数に入る）
    //注意！　OnTriggerStay()と違って剛体同士の衝突判定専用です
    void OnCollisionStay(Collision col)
    {
        if (allowGroundSet)
        {
            //足を向けるべき地形の候補として登録
            Array.Resize(ref touchedGrounds, touchedGrounds.Length + 1);
            touchedGrounds[touchedGrounds.Length - 1] = col.collider;
        }

        GGOOnCollisionStay(col);
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