using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateCharaData")]
public class CharaData : ScriptableObject
{
    public const int totalStatusValue = 3000;
    public const float minLifeRatio = 0.2f;
    public const float maxLifeRatio = 0.8f;

    [System.Serializable]
    public enum EasingType
    {
        easeIn,
        easeOut,
        easeInOut
    }

    [System.Serializable]
    public struct Cursor
    {
        public string name;
        public Vector3 pos;
        public Vector3 direction;
    }
    [System.Serializable]
    public struct MeleeAttackData
    {
        public string name;
        public string cursorName;
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
        public string cursorName;
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
    public struct MeleeAttackKey
    {
        public Vector2 keyFrame;
        public string dataName;
    }
    [System.Serializable]
    public struct ShotKey
    {
        public float keyFrame;
        public string dataName;
    }
    [System.Serializable]
    public struct MoveKey
    {
        public Vector2 keyFrame;
        public Vector3 moveVec;
        public EasingType easingType;
        public float easePow;
    }
    [System.Serializable]
    public struct SEKey
    {
        public float keyFrame;
        public AudioClip se;
    }
    [System.Serializable]
    public struct AttackMotionData
    {
        public string name;
        public int totalFrame;
        public MeleeAttackKey[] meleeAttackKeys;
        public ShotKey[] shotKeys;
        public MoveKey[] moveKeys;
        public MoveKey[] leaderMoveKeys;
        public Vector2[] shieldKeys;
        public SEKey[] seKeys;
    }

    [SerializeField]
    string charaName;
    public string GetCharaName()
    {
        return charaName;
    }

    [SerializeField]
    [Range(minLifeRatio, maxLifeRatio)]
    float lifeRatio = 0.5f;
    public float GetLife()
    {
        return Mathf.Clamp(lifeRatio, minLifeRatio, maxLifeRatio)
        * totalStatusValue;
    }
    public float GetAttackPower()
    {
        return (1 - Mathf.Clamp(lifeRatio, minLifeRatio, maxLifeRatio))
        * totalStatusValue;
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
    AttackMotionData[] attackMotions = { };
    public AttackMotionData[] GetAttackMotions()
    {
        return attackMotions;
    }
    public AttackMotionData SearchAttackMotion(string name)
    {
        for (int i = 0; i < attackMotions.Length; i++)
        {
            if (name == attackMotions[i].name)
            {
                return attackMotions[i];
            }
        }
        return new AttackMotionData();
    }
}
