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

    //?��?��?��?��?��?��?��Z?��?��?��X?��V?��?��?��?��?��^?��C?��~?��?��?��O?��Ŗ�?��t?��?��?��[?��?��?��Ă΂�?��
    //?��?��?��ӁI?��@Update()?��Ƃ͌Ă΂�?��?��?��?��?��?��?��قȂ邽?��ߎ�?��?��?��Y?��?��?��ɂ�?��s?��?��?��ɋC?��?��t?��?��?��ĉ�?��?��?��?��
    protected override void APOUpdate()
    {
        Collider tempGround = currentGround;
        //?��G?��ꂽ?��R?��?��?��C?��_?��[?��̂�?��?��?��ł�?��߂�?��?��?��̂�?��?��U?��?��?��g?��̑�?��?��Ƃ�?��?��
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

        //?��?��?��̃R?��?��?��C?��_?��[?��?��?��?��?��̃R?��?��?��C?��_?��[?��ƃN?��?��?��X?��^?��[?��ɂȂ�?��Ă�?��邩?��?��?��?��
        foreach (MargedGround obj in UnityEngine.Object.FindObjectsOfType<MargedGround>())
        {
            MargedGround.GroundCluster[] groundClusters = obj.GetGroundClusters();
            for (int i = 0; i < groundClusters.Length; i++)
            {
                Collider[] currentColliders = groundClusters[i].colliders;
                for (int j = 0; j < currentColliders.Length; j++)
                {
                    //?��N?��?��?��X?��^?��[?��?��?��?��?���?��?��?��?��?��炻?��̒�?��ōł�?��߂�?��?��?��̂�?��?��U?��?��?��g?��̑�?��?��Ƃ�?��?��
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

        //?��?��?��̃R?��?��?��C?��_?��[?��?��?��?��?��n?��ł�?��?��?��?��̂ł�?��?��ΐ�?��?��?��Ɏ�?��g?��̑�?��?��Ƃ�?��?��
        if (tempGround != null && tempGround.GetComponent<UnLandableObject>() == null)
        {
            currentGround = tempGround;
        }

        //?��G?��ꂽ?��R?��?��?��C?��_?��[?��̏�?��?��?��?��?��Z?��b?��g
        Array.Resize(ref touchedGrounds, 0);

        //?��?��?��?��n?��ʂɌ�?��?��?��?��
        if (currentGround != null
            && currentGround.ClosestPoint(transform.position) != transform.position)
        {
            //?��?��?��?��?��?��?��?��?��?��ׂ�?��ʒu?��?��?��Z?��o?��?��?��?��
            Vector3 localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //?��ǂ�?��炩?��Ƃ�?��?��?��Ώc?��?��?��?��?��ɑ傫?��?��?��?��]?��?��?��?��K?��v?��?��?��?��?��?��Ȃ�
            if (Mathf.Abs(localClosestPoint.z) > Mathf.Abs(localClosestPoint.x))
            {
                //x?��?��?��??�S?��ɂ�?��̈ʒu?��?��?��?��?��?��?��悤?��ɉ�]?��?��?��?��?��?��
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);

                //?��ēx?��?��?��?��?��?��?��?��?��?��ׂ�?��ʒu?��?��?��Z?��o?��?��?��A
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //z?��?��?��??�S?��ɂ�?��̈ʒu?��?��?��?��?��?��?��悤?��ɉ�]?��?��?��?��?��?��
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);
            }
            else
            {
                //z?��?��?��??�S?��ɂ�?��̈ʒu?��?��?��?��?��?��?��悤?��ɉ�]?��?��?��?��?��?��
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);

                //?��ēx?��?��?��?��?��?��?��?��?��?��ׂ�?��ʒu?��?��?��Z?��o?��?��?��A
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //x?��?��?��??�S?��ɂ�?��̈ʒu?��?��?��?��?��?��?��悤?��ɉ�]?��?��?��?��?��?��
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);
            }
        }

        GGOUpdate();
    }

    //?��?��?��̃I?��u?��W?��F?��N?��g?��?��?��R?��?��?��C?��_?��[?��ɐG?��?��Ă�?��?��Ԗ�?��t?��?��?��[?��?��?��?��?��̊֐�?��?��?��Ă΂�?��i?��G?��?��Ă�?��?��R?��?��?��C?��_?��[?��?��?��?��?��?��?��I?��Ɉ�?��?��?��ɓ�?��?��j
    //?��?��?��ӁI?��@OnTriggerStay()?��ƈ�?��?��č�?��̓�?��m?��̏Փ˔�?��?��?��p?��ł�
    void OnCollisionStay(Collision col)
    {
        if (allowGroundSet)
        {
            //?��?��?��?��?��?��?��?��?��?��ׂ�?��n?��`?��̌�?��Ƃ�?��ēo?��^
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