using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BattleField : Field
{
    static BattleField[] allInstances = { };
    public static BattleField[] GetAllInstances()
    {
        return KX_netUtil.CopyArray<BattleField>(allInstances);
    }

    [System.Serializable]
    public struct SpawnData
    {
        public string cassetteID;
        public Vector3 enemyPos;
        public Vector3 enemyRot;
    }
    [System.Serializable]
    public struct Wave
    {
        public SpawnData[] spawnDatas;
    }

    [SerializeField]
    ResourcePalette resourcePalette;
    [SerializeField]
    bool visible;
    [SerializeField]
    string TeamID = "enemy";
    [SerializeField]
    Wave[] waves;

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

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for(int i = 0;i < waves.Length;i++)
        {
            for(int j = 0;j < waves[i].spawnDatas.Length;j++)
            {
                Gizmos.matrix = Matrix4x4.TRS(
                    transform.TransformPoint(waves[i].spawnDatas[j].enemyPos),
                    transform.rotation * Quaternion.Euler(
                    waves[i].spawnDatas[j].enemyRot),
                    Vector3.one);

                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

                Gizmos.matrix = Matrix4x4.identity;
            }
        }

        Gizmos.color = Color.white;
        for(int i = 0;i < waves.Length;i++)
        {
            for(int j = 0;j < waves[i].spawnDatas.Length;j++)
            {
                Handles.Label(
                    transform.TransformPoint(waves[i].spawnDatas[j].enemyPos),
                    waves[i].spawnDatas[j].cassetteID);
            }
        }
    }
#endif

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
                if (wave < waves.Length)
                {
                    for (int i = 0; i < waves[wave].spawnDatas.Length; i++)
                    {
                        LiveEntity.Spawn(resourcePalette,
                            transform.TransformPoint(waves[wave].spawnDatas[i].enemyPos),
                            transform.rotation * Quaternion.Euler(waves[wave].spawnDatas[i].enemyRot),
                            false, TeamID,
                            new string[] { waves[wave].spawnDatas[i].cassetteID }, new int[] { 0 }, 0);
                    }
                    wave++;
                }
                else
                {
                    battled = true;
                }
            }
            annihilated = wave >= waves.Length && livingGuardersNum == 0;
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
            else if (tempLiveEntity.GetTeamID() == TeamID)
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