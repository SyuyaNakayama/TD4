using System;
using UnityEngine;

public class Enemy : CharacterCassette
{
    float TargetSearchRange = 5;

    protected Vector3 targetCursor;

    protected override void CharaUpdate()
    {
        //?申U?申?申?申?申?申?��?申����鐃�?申?申?申��l?申?申?申?申?申?申?申��鐃�?申?申?申?申U?申?申?申?申?申?申?申
        if (!IsAttacking() && GetNearestTarget() != null)
        {
            //?申_?申?申
            TargetAimY(GetNearestTarget().transform.position);
            //?申U?申?申?申?申?申[?申V?申?申?申?申?申?申?申��鐃�
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }
    }

    //?申?申?申��鐃�?申?申LiveEntity?申��鐃�?申?申?申?申G?申?申I?申?申
    public LiveEntity[] GetTargets()
    {
        LiveEntity[] ret = { };
        foreach (LiveEntity obj in LiveEntity.GetAllInstances())
        {
            if (obj.gameObject.activeInHierarchy
                && obj.GetTeamID() != GetLiveEntity().GetTeamID())
            {
                Vector3 localPos =
                KX_netUtil.RelativePosition(
                    GetLiveEntity().transform, obj.transform, Vector3.zero);

                if (Vector3.Magnitude(localPos) <= TargetSearchRange)
                {
                    Array.Resize(ref ret, ret.Length + 1);
                    ret[ret.Length - 1] = obj;
                }
            }
        }

        return ret;
    }

    //?申?申?申��鐃�?申?申LiveEntity?申��鐃�?申?申?申��??申��鐃�I?申?申
    public LiveEntity[] GetFriends()
    {
        LiveEntity[] ret = { };
        foreach (LiveEntity obj in LiveEntity.GetAllInstances())
        {
            if (obj.gameObject.activeInHierarchy
                && obj.GetTeamID() == GetLiveEntity().GetTeamID())
            {
                Vector3 localPos =
                KX_netUtil.RelativePosition(
                    GetLiveEntity().transform, obj.transform, Vector3.zero);

                if (Vector3.Magnitude(localPos) <= TargetSearchRange)
                {
                    Array.Resize(ref ret, ret.Length + 1);
                    ret[ret.Length - 1] = obj;
                }
            }
        }

        return ret;
    }

    //?申��鐃�?申��鐃�?申W?申I?申?申?申���
    public LiveEntity GetNearestTarget()
    {
        LiveEntity ret = null;
        LiveEntity[] targets = GetTargets();

        //?申��鐃�?申��鐃�?申?申?申��鐃�ret?申��鐃�?申?申?申
        for (int i = 0; i < targets.Length; i++)
        {
            if (ret == null ||
            Vector3.Magnitude(targets[i].transform.position - transform.position)
            < Vector3.Magnitude(ret.transform.position - transform.position))
            {
                ret = targets[i];
            }
        }

        return ret;
    }

    //?申W?申I?申��鐃�?申?申?申?申?申?申?申?申?申iY?申?申?申����j
    protected void TargetAimY(Vector3 targetWorldPos, float intensity = 1)
    {
        Quaternion prevRot = GetLiveEntity().transform.rotation;

        Vector3 targetLocalPos = GetLiveEntity().transform.InverseTransformPoint(targetWorldPos);
        GetLiveEntity().transform.Rotate(0, Mathf.Atan2(targetLocalPos.x, targetLocalPos.z) / Mathf.Deg2Rad, 0, Space.Self);

        GetLiveEntity().transform.rotation = Quaternion.Lerp(prevRot, GetLiveEntity().transform.rotation, intensity);
    }
}