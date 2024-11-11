using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : Field
{
    static BattleField[] allInstances = { };
    public static BattleField[] GetAllInstances()
    {
        return KX_netUtil.CopyArray<BattleField>(allInstances);
    }

    const string enemiesTeamCode = "enemy";

    [System.Serializable]
    public struct EnemyAndTransform
    {
        public string cassetteID;
        public Vector3 enemyPos;
        public Vector3 enemyRot;
    }

    [SerializeField]
    ResourcePalette resourcePalette;
    [SerializeField]
    bool visible;
    [SerializeField]
    EnemyAndTransform[] enemies;
    [SerializeField]
    int[] waveEnemyNum;

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
            if (guardersNum == 0)
            {
                int prevWaveEnemyNum = 0;
                for (int i = 0; i < wave; i++)
                {
                    prevWaveEnemyNum += waveEnemyNum[i];
                }
                if (prevWaveEnemyNum < enemies.Length)
                {
                    for (int i = 0; i < waveEnemyNum[wave]; i++)
                    {
                        if (prevWaveEnemyNum + i < enemies.Length)
                        {
                            LiveEntity.Spawn(resourcePalette,
                                transform.TransformPoint(enemies[prevWaveEnemyNum + i].enemyPos),
                                transform.rotation * Quaternion.Euler(enemies[prevWaveEnemyNum + i].enemyRot),
                                false, enemiesTeamCode,
                                new string[] { enemies[prevWaveEnemyNum + i].cassetteID }, new int[] { 0 }, 0);
                        }
                    }
                    wave++;
                }
                else
                {
                    battled = true;
                }
            }
            annihilated = wave >= waveEnemyNum.Length && livingGuardersNum == 0;
            guardersNum = 0;
            livingGuardersNum = 0;
        }
        GetComponent<Collider>().enabled = !battled;
        GetComponent<MeshRenderer>().enabled = (battling || visible) && !battled;
        tempBattling = false;
    }
    protected override void UniqueOnTriggerStay(Collider col)
    {
        LiveEntity tempLiveEntity = col.GetComponent<LiveEntity>();
        if (tempLiveEntity != null)
        {
            if (tempLiveEntity.GetUserControl())
            {
                tempBattling = true;
            }
            else if (tempLiveEntity.GetTeamID() == enemiesTeamCode)
            {
                guardersNum++;
                if (tempLiveEntity.IsLive())
                {
                    livingGuardersNum++;
                }
            }
        }
    }
}