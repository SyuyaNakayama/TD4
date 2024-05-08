using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateCharaData")]
public class CharaData : ScriptableObject
{
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
        public bool grab;
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
        public Vector2[] shieldKeys;
        public SEKey[] seKeys;
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
