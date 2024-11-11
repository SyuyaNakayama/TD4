using System;
using UnityEngine;

public class Enemy : CharacterCassette
{
    float TargetSearchRange = 5;

    protected Vector3 targetCursor;

    protected override void CharaUpdate()
    {
        //?��U?��?��?��?��?��?��?��łȂ�?��?��?��Ɋl?��?��?��?��?��?��?���?��?��?��?��U?��?��?��?��?��?��?��
        if (!IsAttacking() && GetNearestTarget() != null)
        {
            //?��_?��?��
            TargetAimY(GetNearestTarget().transform.position);
            //?��U?��?��?��?��?��[?��V?��?��?��?��?��?��?��Đ�
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }
    }

    //?��?��?���?��?��LiveEntity?��̒�?��?��?��?��G?��?��I?��?��
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

    //?��?��?���?��?��LiveEntity?��̒�?��?��?���??��Ԃ�I?��?��
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

    //?��ł�?��߂�?��W?��I?��?��?��擾
    public LiveEntity GetNearestTarget()
    {
        LiveEntity ret = null;
        LiveEntity[] targets = GetTargets();

        //?��ł�?��߂�?��?��?��̂�ret?��ɓ�?��?��?��
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

    //?��W?��I?��̕�?��?��?��?��?��?��?��?��?��iY?��?��?��̂݁j
    protected void TargetAimY(Vector3 targetWorldPos, float intensity = 1)
    {
        Quaternion prevRot = GetLiveEntity().transform.rotation;

        Vector3 targetLocalPos = GetLiveEntity().transform.InverseTransformPoint(targetWorldPos);
        GetLiveEntity().transform.Rotate(0, Mathf.Atan2(targetLocalPos.x, targetLocalPos.z) / Mathf.Deg2Rad, 0, Space.Self);

        GetLiveEntity().transform.rotation = Quaternion.Lerp(prevRot, GetLiveEntity().transform.rotation, intensity);
    }
}