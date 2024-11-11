using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
[DisallowMultipleComponent]
public class CassetteSlot : MonoBehaviour
{
    [System.Serializable]
    struct IDAndElabled
    {
        public string id;
        public bool enabled;
    }

    public const int teamNum = 10;

    [SerializeField]
    LiveEntity liveEntity;
    [SerializeField]
    string[] inventoryCharaID = { };
    public string[] GetInventoryCharaID()
    {
        string[] ret = new string[inventoryCharaID.Length];
        Array.Copy(inventoryCharaID, ret, inventoryCharaID.Length);
        return ret;
    }
    [SerializeField]
    IDAndElabled[] instantCharaID = { };
    [SerializeField]
    int[] team = { };
    public int[] GetTeam()
    {
        int[] ret = new int[team.Length];
        Array.Copy(team, ret, team.Length);
        return ret;
    }
    float[] life = { };
    public float[] GetLife()
    {
        float[] ret = new float[life.Length];
        Array.Copy(life, ret, life.Length);
        return ret;
    }
    bool restartAble;


    void Awake()
    {
        Array.Resize(ref team, teamNum);
        Array.Resize(ref instantCharaID, teamNum);
        Array.Resize(ref life, teamNum);

        restartAble = true;
    }
    void FixedUpdate()
    {
        restartAble = false;
        Array.Resize(ref team, teamNum);
        Array.Resize(ref instantCharaID, teamNum);
        Array.Resize(ref life, teamNum);

        if (inventoryCharaID.Length <= 0)
        {
            Array.Resize(ref inventoryCharaID, 1);
            inventoryCharaID[0] = "";
        }

        for (int i = 0; i < inventoryCharaID.Length; i++)
        {
            if (inventoryCharaID[i] == null)
            {
                Array.Clear(inventoryCharaID, i, 1);
            }
        }
        List<string> list = new List<string>(inventoryCharaID);
        list.Remove(null);
        inventoryCharaID = list.ToArray();

        restartAble = GetLiveMemberNum() <= 0;

        for (int i = teamNum - 1; i >= 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                if (i == 0 || team[i] < 0)
                {
                    break;
                }
                if (team[i] == team[j])
                {
                    team[i] = -1;
                    break;
                }
            }
        }
    }

    public void Restart(string[] setInventoryCharaID)
    {
        if (restartAble)
        {
            Array.Resize(ref life, teamNum);
            if (setInventoryCharaID.Length != 0)
            {
                inventoryCharaID = setInventoryCharaID;
            }
            for (int i = 0; i < teamNum; i++)
            {
                life[i] = 1;
            }
            restartAble = false;
        }
    }

    public void AddInstantCassette(int cassetteIndex, string cassetteID)
    {
        cassetteIndex =
            Mathf.RoundToInt(Mathf.Repeat(cassetteIndex, instantCharaID.Length));
        instantCharaID[cassetteIndex].id = cassetteID;
        instantCharaID[cassetteIndex].enabled = true;
    }

    public void Restart()
    {
        string[] setInventoryCharaID = { };
        Restart(setInventoryCharaID);
    }

    public bool GetLive(int cassetteIndex)
    {
        return GetEnabled(cassetteIndex) && life[cassetteIndex] >= 0;
    }

    public bool GetEnabled(int cassetteIndex)
    {
        return team[cassetteIndex] >= 0;
    }

    public int GetLiveMemberNum()
    {
        int ret = 0;
        for (int i = 0; i < teamNum; i++)
        {
            if (GetLive(i))
            {
                ret++;
            }
        }
        return ret;
    }

    public int GetEnabledMemberNum()
    {
        int ret = 0;
        for (int i = 0; i < teamNum; i++)
        {
            if (GetEnabled(i))
            {
                ret++;
            }
        }
        return ret;
    }

    public void Load()
    {
        if (restartAble)
        {
            if (liveEntity.GetUserControl())
            {
                string path = Application.persistentDataPath + "/savedata.json";
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    SaveData data = JsonUtility.FromJson<SaveData>(json);

                    life = data.life;
                    team = data.teamCharaIndex;
                    inventoryCharaID = data.inventoryCharaID;
                    for (int i = 0; i < inventoryCharaID.Length; i++)
                    {
                        inventoryCharaID[i] = data.inventoryCharaID[i];
                    }
                }
                else
                {
                    Restart();
                }
            }
            else
            {
                Restart();
            }
            restartAble = false;
        }
    }

    public string GetTeamCharaID(int cassetteIndex)
    {
        if (instantCharaID[cassetteIndex].enabled)
        {
            return instantCharaID[cassetteIndex].id;
        }
        return inventoryCharaID[team[cassetteIndex]];
    }
}