using System;
using UnityEngine;

public class Enemy : LiveEntity
{
    [SerializeField]
    Sensor sensor;

    //������LiveEntity�̒�����W�I��I��
    LiveEntity[] GetTargets()
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
}