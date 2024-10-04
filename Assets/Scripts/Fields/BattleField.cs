using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : Field
{
    static BattleField[] allInstances = { };
    public static BattleField[] GetAllInstances()
    {
        BattleField[] ret = new BattleField[allInstances.Length];
        Array.Copy(allInstances, ret, allInstances.Length);
        return ret;
    }

    const string teamID = "enemy";
    const int maxLifeTime = 5;

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
    int lifeTime;
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
        //全インスタンスを入れる変数を更新
        List<BattleField> allInstancesList =
            new List<BattleField>(allInstances);
        allInstancesList.RemoveAll(where => !where || where == this);
        allInstances = allInstancesList.ToArray();
        Array.Resize(ref allInstances, allInstances.Length + 1);
        allInstances[allInstances.Length - 1] = this;

        battling = tempBattling;
        if (battling)
        {
            if (wave >= guarders.Length && livingGuardersNum == 0)
            {
                lifeTime = Mathf.Max(0, lifeTime - 1);
            }
            else
            {
                lifeTime = maxLifeTime;
            }

            annihilated = lifeTime <= 0;
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
        else
        {
            lifeTime = maxLifeTime;
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
