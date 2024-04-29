using System;
using UnityEngine;

public class Enemy : LiveEntity
{
    [SerializeField]
    Sensor sensor;

    //見つけたLiveEntityの中から標的を選別
    public LiveEntity[] GetTargets()
    {
        LiveEntity[] ret = { };
        LiveEntity[] detectedLiveEntities = sensor.GetTargets();

        //teamIDが違うものだけ選別
        for (int i = 0; i < detectedLiveEntities.Length; i++)
        {
            if (detectedLiveEntities[i].GetTeamID() != GetTeamID())
            {
                //配列に代入
                Array.Resize(ref ret, ret.Length + 1);
                ret[ret.Length - 1] = detectedLiveEntities[i];
            }
        }

        return ret;
    }

    //最も近い標的を取得
    public LiveEntity GetNearestTarget()
    {
        LiveEntity ret = null;
        LiveEntity[] targets = GetTargets();

        //最も近いものをretに入れる
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