using System;
using UnityEngine;

public class Enemy : LiveEntity
{
    [SerializeField]
    Sensor sensor;
    protected Vector3 targetCursor;

    protected override void LiveEntityUpdate()
    {
        //�U�����쒆�łȂ����Ɋl������������U�������
        if (!IsAttacking() && GetNearestTarget() != null)
        {
            //�_��
            targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position);
            transform.Rotate(0,
                Mathf.Atan2(targetCursor.x, targetCursor.z) / Mathf.Deg2Rad,
                0, Space.Self);
            //�U�����[�V�������Đ�
            SetAttackMotion(GetData().GetDefaultAttackMotionName());
        }
    }

    //������LiveEntity�̒�����W�I��I��
    public LiveEntity[] GetTargets()
    {
        LiveEntity[] ret = { };
        LiveEntity[] detectedLiveEntities = sensor.GetTargets();

        //teamID���Ⴄ���̂����I��
        for (int i = 0; i < detectedLiveEntities.Length; i++)
        {
            if (detectedLiveEntities[i].GetTeamID() != GetTeamID())
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
}