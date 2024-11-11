using System;
using UnityEngine;

public class Enemy : CharacterCassette
{
    [SerializeField]
    Sensor sensor;
    protected Vector3 targetCursor;

    protected override void CharaUpdate()
    {
        //�U�����쒆�łȂ����Ɋl������������U�������
        if (!IsAttacking() && GetNearestTarget() != null)
        {
            //�_��
            TargetAimY(GetNearestTarget().transform.position);
            //�U�����[�V�������Đ�
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }
    }

    //������LiveEntity�̒�����G��I��
    public LiveEntity[] GetTargets()
    {
        LiveEntity[] ret = { };
        LiveEntity[] detectedLiveEntities = sensor.GetTargets();

        //teamID���Ⴄ���̂����I��
        for (int i = 0; i < detectedLiveEntities.Length; i++)
        {
            if (detectedLiveEntities[i].GetTeamID() != GetLiveEntity().GetTeamID())
            {
                //�z��ɒǉ�
                Array.Resize(ref ret, ret.Length + 1);
                ret[ret.Length - 1] = detectedLiveEntities[i];
            }
        }

        return ret;
    }

    //������LiveEntity�̒����璇�Ԃ�I��
    public LiveEntity[] GetFriends()
    {
        LiveEntity[] ret = { };
        LiveEntity[] detectedLiveEntities = sensor.GetTargets();

        //teamID���������̂����I��
        for (int i = 0; i < detectedLiveEntities.Length; i++)
        {
            if (detectedLiveEntities[i].GetTeamID() == GetLiveEntity().GetTeamID())
            {
                //�z��ɑ��
                Array.Resize(ref ret, ret.Length + 1);
                ret[ret.Length - 1] = detectedLiveEntities[i];
            }
        }

        return ret;
    }

    //�ł��߂��W�I���擾
    public LiveEntity GetNearestTarget()
    {
        LiveEntity ret = null;
        LiveEntity[] targets = GetTargets();

        //�ł��߂����̂�ret�ɓ����
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

    //�W�I�̕����������iY���̂݁j
    protected void TargetAimY(Vector3 targetWorldPos, float intensity = 1)
    {
        //�_��
        Vector3 targetLocalPos = transform.InverseTransformPoint(
            targetWorldPos);
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            transform.localRotation *
            Quaternion.Euler(new Vector3(
            0,
            Mathf.Atan2(targetLocalPos.x, targetLocalPos.z) / Mathf.Deg2Rad,
            0)),
            intensity);
    }
}