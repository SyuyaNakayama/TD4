using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : Field
{
    const string teamID = "enemy";

    [System.Serializable]
    public struct GuarderAndTransform
    {
        public LiveEntity liveEntity;
        public Vector3 position;
        public Vector3 eulerAngles;
    }
    [System.Serializable]
    public struct GuardersTeamData
    {
        public GuarderAndTransform[] data;
    }

    [SerializeField]
    bool visible;
    [SerializeField]
    GuardersTeamData[] guarders;

    bool tempBattling;
    bool battling;
    public bool GetBattling()
    {
        return battling;
    }
    bool battled;
    public bool GetBattled()
    {
        return battled;
    }
    bool annihilated;
    public bool GetAnnihilated()
    {
        return annihilated;
    }
    int wave;
    int waveWait;
    int guardersNum;
    int livingGuardersNum;
    void Awake()
    {
        waveWait = 10;
        tempBattling = false;
        battling = false;
        battled = false;
        wave = 0;
    }
    protected override void UniqueFixedUpdate()
    {
        battling = tempBattling;
        if (battling)
        {
            annihilated = wave >= guarders.Length && livingGuardersNum == 0;
            if (guardersNum == 0)
            {
                if (wave < guarders.Length)
                {
                    for (int i = 0; i < guarders[wave].data.Length; i++)
                    {
                        GuarderAndTransform current = guarders[wave].data[i];
                        LiveEntity.Spawn(current.liveEntity,
                            transform.TransformPoint(current.position),
                            transform.rotation * Quaternion.Euler(current.eulerAngles),
                            teamID);
                    }
                    wave++;
                }
                else
                {
                    battled = true;
                }
            }
            guardersNum = 0;
            livingGuardersNum = 0;
        }
        GetComponent<Collider>().enabled = !battled;
        GetComponent<MeshRenderer>().enabled = (battling || visible) && !battled;
        tempBattling = false;
    }
    protected override void UniqueOnTriggerStay(Collider col)
    {
        LiveEntity currentTarget = col.GetComponent<LiveEntity>();
        if (currentTarget != null)
        {
            if (currentTarget.GetTeamID() != teamID)
            {
                tempBattling = true;
            }
            else
            {
                guardersNum++;
                if (currentTarget.IsLive())
                {
                    livingGuardersNum++;
                }
            }
        }
    }
}
