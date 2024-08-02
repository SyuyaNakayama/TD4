using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoGroObject : AlhaPhysicsObject
{
    [SerializeField]
    Collider currentGround;
    public Collider GetCurrentGeround()
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

    //?申?申?申?申?申?申?申Z?申?申?申X?申V?申?申?申?申?申^?申C?申~?申?申?申O?申��鐃�?申t?申?申?申[?申?申?申����鐃�?申
    //?申?申?申��I?申@Update()?申��������鐃�?申?申?申?申?申?申?申�������?申��鐃�?申?申?申Y?申?申?申��鐃�?申s?申?��?申��C?申?申t?申?申?申��鐃�?申?申?申?申
    protected override void APOUpdate()
    {
        Collider tempGround = currentGround;
        //?申G?申���?申R?申?申?申C?申_?申[?申��鐃�?申?申?申��鐃�?申��鐃�?申?申?申��鐃�?申?申U?申?申?申g?申��鐃�?申?申��鐃�?申?申
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

        //?申?申?申��R?申?申?申C?申_?申[?申?申?申?申?申��R?申?申?申C?申_?申[?申��N?申?申?申X?申^?申[?申����鐃�?申��鐃�?申���?申?申?申?申
        foreach (MargedGround obj in UnityEngine.Object.FindObjectsOfType<MargedGround>())
        {
            MargedGround.GroundCluster[] groundClusters = obj.GetGroundClusters();
            for (int i = 0; i < groundClusters.Length; i++)
            {
                Collider[] currentColliders = groundClusters[i].colliders;
                for (int j = 0; j < currentColliders.Length; j++)
                {
                    //?申N?申?申?申X?申^?申[?申?申?申?申?申��鐃�?申?申?申?申?申���?申��鐃�?申����鐃�?申��鐃�?申?申?申��鐃�?申?申U?申?申?申g?申��鐃�?申?申��鐃�?申?申
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

        //?申?申?申��R?申?申?申C?申_?申[?申?申?申?申?申n?申��鐃�?申?申?申?申����鐃�?申?申��鐃�?申?申?申��鐃�?申g?申��鐃�?申?申��鐃�?申?申
        if (tempGround != null && tempGround.GetComponent<UnLandableObject>() == null)
        {
            currentGround = tempGround;
        }

        //?申G?申���?申R?申?申?申C?申_?申[?申��鐃�?申?申?申?申?申Z?申b?申g
        Array.Resize(ref touchedGrounds, 0);

        //?申?申?申?申n?申����鐃�?申?申?申?申
        if (currentGround != null
            && currentGround.ClosestPoint(transform.position) != transform.position)
        {
            //?申?申?申?申?申?申?申?申?申?申��鐃�?申��u?申?申?申Z?申o?申?申?申?申
            Vector3 localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //?申��鐃�?申���?申��鐃�?申?申?申��c?申?申?申?申?申�����?申?申?申?申]?申?申?申?申K?申v?申?申?申?申?申?申��鐃�
            if (Mathf.Abs(localClosestPoint.z) > Mathf.Abs(localClosestPoint.x))
            {
                //x?申?申?申??�S?申��鐃�?申����u?申?申?申?申?申?申?申���?申��鐃�]?申?申?申?申?申?申
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);

                //?申��x?申?申?申?申?申?申?申?申?申?申��鐃�?申��u?申?申?申Z?申o?申?申?申A
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //z?申?申?申??�S?申��鐃�?申����u?申?申?申?申?申?申?申���?申��鐃�]?申?申?申?申?申?申
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);
            }
            else
            {
                //z?申?申?申??�S?申��鐃�?申����u?申?申?申?申?申?申?申���?申��鐃�]?申?申?申?申?申?申
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);

                //?申��x?申?申?申?申?申?申?申?申?申?申��鐃�?申��u?申?申?申Z?申o?申?申?申A
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //x?申?申?申??�S?申��鐃�?申����u?申?申?申?申?申?申?申���?申��鐃�]?申?申?申?申?申?申
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);
            }
        }

        GGOUpdate();
    }

    //?申?申?申��I?申u?申W?申F?申N?申g?申?申?申R?申?申?申C?申_?申[?申��G?申?申��鐃�?申?申��鐃�?申t?申?申?申[?申?申?申?申?申����鐃�?申?申?申����鐃�?申i?申G?申?申��鐃�?申?申R?申?申?申C?申_?申[?申?申?申?申?申?申?申I?申��鐃�?申?申?申��鐃�?申?申j
    //?申?申?申��I?申@OnTriggerStay()?申��鐃�?申?申��鐃�?申��鐃�?申m?申������鐃�?申?申?申p?申��鐃�
    void OnCollisionStay(Collision col)
    {
        if (allowGroundSet)
        {
            //?申?申?申?申?申?申?申?申?申?申��鐃�?申n?申`?申��鐃�?申��鐃�?申��o?申^
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
        currentGround = setCurrentGround;
    }
}