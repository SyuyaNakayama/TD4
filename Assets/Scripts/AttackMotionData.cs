using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateAttackMotionData")]
public class AttackMotionData : ScriptableObject
{
    [System.Serializable]
    public struct Cursor
    {
        public string name;
        public Vector3 pos;
        public Vector3 direction;
    }
    [System.Serializable]
    public struct AttackData
    {
        public float power;
        public bool vectorBlow;
        public float blowForce;
        public int hitback;
    }
    [System.Serializable]
    public struct MeleeAttackData
    {
        public string name;
        public float power;
        public float scale;
        public bool vectorBlow;
        public float blowForce;
        public int hitback;
    }
    [System.Serializable]
    public struct ShotData
    {
        public string name;
        public float power;
        public float scale;
        public float speed;
        public int lifetime;
        public float blowForce;
        public int hitback;
        public bool yBillBoard;
        public bool flatBillBoard;
        public Sprite sprite;
    }
    [System.Serializable]
    public struct AttackKey
    {
        public Vector2 keyFrame;
        public string dataName;
        public string cursorName;
    }
    [System.Serializable]
    public struct MoveKey
    {
        public Vector2 keyFrame;
        public Vector3 moveVec;
        public KX_netUtil.EaseType easeType;
        public float easePow;
    }
    [System.Serializable]
    public struct SEKey
    {
        public float keyFrame;
        public AudioClip se;
    }
    [System.Serializable]
    public struct Data
    {
        public string name;
        public int totalFrame;
        public AttackKey[] meleeAttackKeys;
        public AttackKey[] shotKeys;
        public MoveKey[] moveKeys;
        public Vector2[] shieldKeys;
        public Vector2[] disAllowGroundSetKeys;
        public SEKey[] seKeys;
    }
    [SerializeField]
    Cursor[] cursors = { };
    public Cursor[] GetCursors()
    {
        return cursors;
    }
    public int SearchCursorIndex(string name)
    {
        for (int i = 0; i < cursors.Length; i++)
        {
            if (name == cursors[i].name)
            {
                return i;
            }
        }
        return -1;
    }

    [SerializeField]
    MeleeAttackData[] meleeAttackDatas = { };
    public MeleeAttackData SearchMeleeAttackData(string name)
    {
        for (int i = 0; i < meleeAttackDatas.Length; i++)
        {
            if (name == meleeAttackDatas[i].name)
            {
                return meleeAttackDatas[i];
            }
        }
        return new MeleeAttackData();
    }

    [SerializeField]
    ShotData[] shotDatas = { };
    public ShotData SearchShotData(string name)
    {
        for (int i = 0; i < shotDatas.Length; i++)
        {
            if (name == shotDatas[i].name)
            {
                return shotDatas[i];
            }
        }
        return new ShotData();
    }

    [SerializeField]
    Data data;
    public Data GetData()
    {
        return data;
    }
}
