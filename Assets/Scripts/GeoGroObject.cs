using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoGroObject : AlhaPhysicsObject
{
    [SerializeField]
    protected bool dataLock = true;
    [SerializeField]
    Collider currentGround;
    public Collider GetCurrentGround()
    {
        return currentGround;
    }
    Collider[] touchedGrounds = { };
    [SerializeField]
    protected bool allowGroundSet = true;

    protected override void APOAwake()
    {
        GGOAwake();
    }

    //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽZ?ｿｽ?ｿｽ?ｿｽX?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ^?ｿｽC?ｿｽ~?ｿｽ?ｿｽ?ｿｽO?ｿｽﾅ厄ｿｽ?ｿｽt?ｿｽ?ｿｽ?ｿｽ[?ｿｽ?ｿｽ?ｿｽﾄばゑｿｽ?ｿｽ
    //?ｿｽ?ｿｽ?ｿｽﾓ！?ｿｽ@Update()?ｿｽﾆは呼ばゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾙなるた?ｿｽﾟ趣ｿｽ?ｿｽ?ｿｽ?ｿｽY?ｿｽ?ｿｽ?ｿｽﾉゑｿｽ?ｿｽs?ｿｽ?合?ｿｽﾉ気?ｿｽ?ｿｽt?ｿｽ?ｿｽ?ｿｽﾄ会ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
    protected override void APOUpdate()
    {
        Collider tempGround = currentGround;
        //?ｿｽG?ｿｽ黷ｽ?ｿｽR?ｿｽ?ｿｽ?ｿｽC?ｿｽ_?ｿｽ[?ｿｽﾌゑｿｽ?ｿｽ?ｿｽ?ｿｽﾅゑｿｽ?ｿｽﾟゑｿｽ?ｿｽ?ｿｽ?ｿｽﾌゑｿｽ?ｿｽ?ｿｽU?ｿｽ?ｿｽ?ｿｽg?ｿｽﾌ托ｿｽ?ｿｽ?ｿｽﾆゑｿｽ?ｿｽ?ｿｽ
        float nearestGroundDistance = 0;
        bool detected = false;
        for (int i = 0; i < touchedGrounds.Length; i++)
        {
            if (touchedGrounds[i] != null)
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
        }

        //?ｿｽ?ｿｽ?ｿｽﾌコ?ｿｽ?ｿｽ?ｿｽC?ｿｽ_?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌコ?ｿｽ?ｿｽ?ｿｽC?ｿｽ_?ｿｽ[?ｿｽﾆク?ｿｽ?ｿｽ?ｿｽX?ｿｽ^?ｿｽ[?ｿｽﾉなゑｿｽ?ｿｽﾄゑｿｽ?ｿｽ驍ｩ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
        foreach (MargedGround obj in UnityEngine.Object.FindObjectsOfType<MargedGround>())
        {
            MargedGround.GroundCluster[] groundClusters = obj.GetGroundClusters();
            for (int i = 0; i < groundClusters.Length; i++)
            {
                Collider[] currentColliders = groundClusters[i].colliders;
                for (int j = 0; j < currentColliders.Length; j++)
                {
                    //?ｿｽN?ｿｽ?ｿｽ?ｿｽX?ｿｽ^?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾂゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ轤ｻ?ｿｽﾌ抵ｿｽ?ｿｽﾅ最ゑｿｽ?ｿｽﾟゑｿｽ?ｿｽ?ｿｽ?ｿｽﾌゑｿｽ?ｿｽ?ｿｽU?ｿｽ?ｿｽ?ｿｽg?ｿｽﾌ托ｿｽ?ｿｽ?ｿｽﾆゑｿｽ?ｿｽ?ｿｽ
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
                                    && KX_netUtil.IsInsidePosition(
                                    currentColliders[k], tempGround.ClosestPoint(transform.position)))
                                {
                                    if (currentColliders[k].GetComponent<UnLandableObject>() == null)
                                    {
                                        tempGround = currentColliders[k];
                                        nearestGroundDistance = currentGroundDistance;
                                        detected = true;
                                    }
                                    else
                                    {
                                        for (int l = 0; l < currentColliders.Length; l++)
                                        {
                                            if (currentColliders[l] != null)
                                            {
                                                currentGroundDistance = Vector3.Magnitude(
                                                    currentColliders[l].ClosestPoint(transform.position) - transform.position);
                                                if ((!detected || currentGroundDistance < nearestGroundDistance)
                                                    && KX_netUtil.IsHit(currentColliders[k], currentColliders[l], 0)
                                                    && currentColliders[l].GetComponent<UnLandableObject>() == null)
                                                {
                                                    tempGround = currentColliders[l];
                                                    nearestGroundDistance = currentGroundDistance;
                                                    detected = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        //?ｿｽ?ｿｽ?ｿｽﾌコ?ｿｽ?ｿｽ?ｿｽC?ｿｽ_?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽn?ｿｽﾅゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌでゑｿｽ?ｿｽ?ｿｽﾎ撰ｿｽ?ｿｽ?ｿｽ?ｿｽﾉ趣ｿｽ?ｿｽg?ｿｽﾌ托ｿｽ?ｿｽ?ｿｽﾆゑｿｽ?ｿｽ?ｿｽ
        if (tempGround != null && tempGround.GetComponent<UnLandableObject>() == null)
        {
            currentGround = tempGround;
        }

        //?ｿｽG?ｿｽ黷ｽ?ｿｽR?ｿｽ?ｿｽ?ｿｽC?ｿｽ_?ｿｽ[?ｿｽﾌ擾ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽZ?ｿｽb?ｿｽg
        Array.Resize(ref touchedGrounds, 0);

        //?ｿｽ?ｿｽ?ｿｽ?ｿｽn?ｿｽﾊに鯉ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
        if (currentGround != null
            && currentGround.ClosestPoint(transform.position) != transform.position)
        {
            //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾗゑｿｽ?ｿｽﾊ置?ｿｽ?ｿｽ?ｿｽZ?ｿｽo?ｿｽ?ｿｽ?ｿｽ?ｿｽ
            Vector3 localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //?ｿｽﾇゑｿｽ?ｿｽ轤ｩ?ｿｽﾆゑｿｽ?ｿｽ?ｿｽ?ｿｽﾎ縦?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾉ大き?ｿｽ?ｿｽ?ｿｽ?ｿｽ]?ｿｽ?ｿｽ?ｿｽ?ｿｽK?ｿｽv?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾈゑｿｽ
            if (Mathf.Abs(localClosestPoint.z) > Mathf.Abs(localClosestPoint.x))
            {
                //x?ｿｽ?ｿｽ?ｿｽ??心?ｿｽﾉゑｿｽ?ｿｽﾌ位置?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ謔､?ｿｽﾉ会ｿｽ]?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);

                //?ｿｽﾄ度?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾗゑｿｽ?ｿｽﾊ置?ｿｽ?ｿｽ?ｿｽZ?ｿｽo?ｿｽ?ｿｽ?ｿｽA
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //z?ｿｽ?ｿｽ?ｿｽ??心?ｿｽﾉゑｿｽ?ｿｽﾌ位置?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ謔､?ｿｽﾉ会ｿｽ]?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);
            }
            else
            {
                //z?ｿｽ?ｿｽ?ｿｽ??心?ｿｽﾉゑｿｽ?ｿｽﾌ位置?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ謔､?ｿｽﾉ会ｿｽ]?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);

                //?ｿｽﾄ度?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾗゑｿｽ?ｿｽﾊ置?ｿｽ?ｿｽ?ｿｽZ?ｿｽo?ｿｽ?ｿｽ?ｿｽA
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //x?ｿｽ?ｿｽ?ｿｽ??心?ｿｽﾉゑｿｽ?ｿｽﾌ位置?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ謔､?ｿｽﾉ会ｿｽ]?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);
            }
        }

        GGOUpdate();
    }

    //?ｿｽ?ｿｽ?ｿｽﾌオ?ｿｽu?ｿｽW?ｿｽF?ｿｽN?ｿｽg?ｿｽ?ｿｽ?ｿｽR?ｿｽ?ｿｽ?ｿｽC?ｿｽ_?ｿｽ[?ｿｽﾉ触?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽﾔ厄ｿｽ?ｿｽt?ｿｽ?ｿｽ?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ関撰ｿｽ?ｿｽ?ｿｽ?ｿｽﾄばゑｿｽ?ｿｽi?ｿｽG?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽR?ｿｽ?ｿｽ?ｿｽC?ｿｽ_?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽI?ｿｽﾉ茨ｿｽ?ｿｽ?ｿｽ?ｿｽﾉ難ｿｽ?ｿｽ?ｿｽj
    //?ｿｽ?ｿｽ?ｿｽﾓ！?ｿｽ@OnTriggerStay()?ｿｽﾆ茨ｿｽ?ｿｽ?ｿｽﾄ搾ｿｽ?ｿｽﾌ難ｿｽ?ｿｽm?ｿｽﾌ衝突費ｿｽ?ｿｽ?ｿｽ?ｿｽp?ｿｽﾅゑｿｽ
    void OnCollisionStay(Collision col)
    {
        if (allowGroundSet)
        {
            //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾗゑｿｽ?ｿｽn?ｿｽ`?ｿｽﾌ鯉ｿｽ?ｿｽﾆゑｿｽ?ｿｽﾄ登?ｿｽ^
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

    public void SetCurrentGround(Collider setCurrentGround)
    {
        if (!dataLock)
        {
            currentGround = setCurrentGround;
        }
    }
    public void SetAllowGroundSet(bool setAllowGroundSet)
    {
        if (!dataLock)
        {
            allowGroundSet = setAllowGroundSet;
        }
    }
}