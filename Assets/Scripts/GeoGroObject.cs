using System;
using UnityEngine;

public class GeoGroObject : AlhaPhysicsObject
{
    [SerializeField]
    protected bool dataLock = true;
    [SerializeField]
    protected Collider currentGround;
    public Collider GetCurrentGround()
    {
        return currentGround;
    }
    Collider[] touchedGrounds = { };
    Collider[] touchedTriggers = { };
    [SerializeField]
    protected bool allowGroundSet = true;

    protected override void APOAwake()
    {
        GGOAwake();
    }

    protected override void APOUpdate()
    {
        Collider tempGround = currentGround;
        //最も近い足場を見つける
        float nearestGroundDistance = 0;
        bool detected = false;
        for (int i = 0; i < touchedGrounds.Length; i++)
        {
            if (touchedGrounds[i])
            {
                float currentGroundDistance = Vector3.Magnitude(
                touchedGrounds[i].ClosestPoint(transform.position) - transform.position);
                if (!detected || currentGroundDistance < nearestGroundDistance)
                {
                    tempGround = touchedGrounds[i];
                    nearestGroundDistance = currentGroundDistance;
                    detected = true;
                }
            }
        }

        //??????????R?????????C???_???[????????????????R?????????C???_???[????N?????????X???^???[?????????ﾄ??????????????????
        foreach (MargedGround obj in MargedGround.GetAllInstances())
        {
            MargedGround.GroundCluster[] groundClusters = obj.GetGroundClusters();
            for (int i = 0; i < groundClusters.Length; i++)
            {
                Collider[] currentColliders = groundClusters[i].colliders;
                for (int j = 0; j < currentColliders.Length; j++)
                {
                    //???N?????????X???^???[???????????????ﾂ・?????????????????????????ﾅ????????????????????????U?????????g???????????????????
                    if (currentColliders[j] == tempGround)
                    {
                        detected = false;
                        for (int k = 0; k < currentColliders.Length; k++)
                        {
                            if (currentColliders[k] && tempGround)
                            {
                                float currentGroundDistance = Vector3.Magnitude(
                                    currentColliders[k].ClosestPoint(transform.position) - transform.position);
                                if ((!detected || currentGroundDistance < nearestGroundDistance)
                                    && KX_netUtil.IsInsidePosition(
                                    currentColliders[k], tempGround.ClosestPoint(transform.position)))
                                {
                                    if (IsLandableGround(currentColliders[k]))
                                    {
                                        tempGround = currentColliders[k];
                                        nearestGroundDistance = currentGroundDistance;
                                        detected = true;
                                    }
                                    else
                                    {
                                        bool needSearchGround = true;
                                        for (int l = 0; l < currentColliders.Length; l++)
                                        {
                                            if (currentColliders[l] != currentColliders[k]
                                                && currentColliders[l] == currentGround)
                                            {
                                                needSearchGround = false;
                                            }
                                        }
                                        if (needSearchGround)
                                        {
                                            for (int l = 0; l < currentColliders.Length; l++)
                                            {
                                                if (currentColliders[l])
                                                {
                                                    currentGroundDistance = Vector3.Magnitude(
                                                        currentColliders[l].ClosestPoint(transform.position) - transform.position);
                                                    if ((!detected || currentGroundDistance < nearestGroundDistance)
                                                        && IsLandableGround(currentColliders[l]))
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
                        }
                        break;
                    }
                }
            }
        }

        //着地できるものなら地面として登録
        if (tempGround && IsLandableGround(tempGround))
        {
            currentGround = tempGround;
        }

        //配列リセット
        Array.Resize(ref touchedGrounds, 0);
        Array.Resize(ref touchedTriggers, 0);

        //地面に足を向ける
        if (currentGround
            && currentGround.ClosestPoint(transform.position) != transform.position)
        {
            //????????????????????????????????????u?????????Z???o????????????
            Vector3 localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //????????????????????????c?????????????????????????????]????????????K???v????????????????????
            if (Mathf.Abs(localClosestPoint.z) > Mathf.Abs(localClosestPoint.x))
            {
                //x????????????S??????????u???????????????????????????]??????????????????
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);

                //????x????????????????????????????????????u?????????Z???o?????????A
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //z????????????S??????????u???????????????????????????]??????????????????
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);
            }
            else
            {
                //z????????????S??????????u???????????????????????????]??????????????????
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);

                //????x????????????????????????????????????u?????????Z???o?????????A
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //x????????????S??????????u???????????????????????????]??????????????????
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);
            }
        }

        GGOUpdate();
    }

    void OnCollisionStay(Collision col)
    {
        if (allowGroundSet)
        {
            Array.Resize(ref touchedGrounds, touchedGrounds.Length + 1);
            touchedGrounds[touchedGrounds.Length - 1] = col.collider;
        }

        GGOOnCollisionStay(col);
    }

    void OnTriggerStay(Collider col)
    {
        if (allowGroundSet)
        {
            Array.Resize(ref touchedTriggers, touchedTriggers.Length + 1);
            touchedTriggers[touchedTriggers.Length - 1] = col;
        }

        GGOOnTriggerStay(col);
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
    protected virtual void GGOOnTriggerStay(Collider col)
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

    bool IsLandableGround(Collider col)
    {
        return col.enabled && col.gameObject.activeInHierarchy
            && !col.GetComponent<UnLandableObject>()
            && !col.isTrigger;
    }
}