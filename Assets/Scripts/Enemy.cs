using System;
using UnityEngine;

public class Enemy : CharacterCassette
{
    float TargetSearchRange = 5;

    protected Vector3 targetCursor;

    protected override void CharaUpdate()
    {
        //?øΩU?øΩ?øΩ?øΩ?øΩ?øΩ?íÜ?øΩ≈Ç»ÇÔøΩ?øΩ?øΩ?øΩ…äl?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ¬ÇÔøΩ?øΩ?øΩ?øΩ?øΩU?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ
        if (!IsAttacking() && GetNearestTarget() != null)
        {
            //?øΩ_?øΩ?øΩ
            TargetAimY(GetNearestTarget().transform.position);
            //?øΩU?øΩ?øΩ?øΩ?øΩ?øΩ[?øΩV?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩƒêÔøΩ
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }
    }

    //?øΩ?øΩ?øΩ¬ÇÔøΩ?øΩ?øΩLiveEntity?øΩÃíÔøΩ?øΩ?øΩ?øΩ?øΩG?øΩ?øΩI?øΩ?øΩ
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

    //?øΩ?øΩ?øΩ¬ÇÔøΩ?øΩ?øΩLiveEntity?øΩÃíÔøΩ?øΩ?øΩ?øΩÁí??øΩ‘ÇÔøΩI?øΩ?øΩ
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

    //?øΩ≈ÇÔøΩ?øΩﬂÇÔøΩ?øΩW?øΩI?øΩ?øΩ?øΩÊìæ
    public LiveEntity GetNearestTarget()
    {
        LiveEntity ret = null;
        LiveEntity[] targets = GetTargets();

        //?øΩ≈ÇÔøΩ?øΩﬂÇÔøΩ?øΩ?øΩ?øΩÃÇÔøΩret?øΩ…ìÔøΩ?øΩ?øΩ?øΩ
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

    //?øΩW?øΩI?øΩÃïÔøΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩiY?øΩ?øΩ?øΩÃÇ›Åj
    protected void TargetAimY(Vector3 targetWorldPos, float intensity = 1)
    {
        Quaternion prevRot = GetLiveEntity().transform.rotation;

        Vector3 targetLocalPos = GetLiveEntity().transform.InverseTransformPoint(targetWorldPos);
        GetLiveEntity().transform.Rotate(0, Mathf.Atan2(targetLocalPos.x, targetLocalPos.z) / Mathf.Deg2Rad, 0, Space.Self);

        GetLiveEntity().transform.rotation = Quaternion.Lerp(prevRot, GetLiveEntity().transform.rotation, intensity);
    }
}