using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateAttackMotionData")]
public class AttackMotionData : ScriptableObject
{
    public enum TriggerInputType
    {
        tap,
        preMove,
        move,
        postMove,
        preWalk,
        walk,
        postWalk,
        jump,
        nutral,
    }

    [Serializable]
    public struct Cursor
    {
        public string name;
        public Vector3 pos;
        public Vector3 direction;
    }
    [Serializable]
    public struct CursorSweepData
    {
        public string cursorName;
        public string startCursorName;
        public string endCursorName;
        public KX_netUtil.EaseType easeType;
        public float easePow;
    }
    [Serializable]
    public struct AttackData
    {
        public float power;
        public bool vectorBlow;
        public float blowForce;
        public int hitback;
        public int ghostTime;
    }
    [Serializable]
    public struct ProjectileData
    {
        public float speed;
        public int lifetime;
        public bool setGround;
    }
    [Serializable]
    public struct BillboardData
    {
        public Sprite sprite;
        public bool yBillboard;
        public bool flatBillboard;
    }

    [Serializable]
    public struct MeleeAttackData
    {
        public string name;
        public AttackData attackData;
        public float scale;
        public BillboardData billboardData;
    }
    [Serializable]
    public struct ShotData
    {
        public string name;
        public AttackData attackData;
        public float scale;
        public BillboardData billboardData;
        public ProjectileData projectileData;
    }
    [Serializable]
    public struct AttackKey
    {
        public Vector2 keyFrame;
        public string dataName;
        public string cursorName;
        public bool postMove;
    }
    [Serializable]
    public struct SummonKey
    {
        public float keyFrame;
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        public string[] inventoryCharaID;
        public int[] teamMember;
        public int cassetteIndex;
    }
    [Serializable]
    public struct MoveKey
    {
        public Vector2 keyFrame;
        public Vector3 moveVec;
        public KX_netUtil.AxisSwitch ignoreAxis;
        public KX_netUtil.EaseType easeType;
        public float easePow;
    }
    [Serializable]
    public struct ImpulseMoveKey
    {
        public Vector2 keyFrame;
        public Vector3 moveVec;
        public KX_netUtil.AxisSwitch ignoreAxis;
    }
    [Serializable]
    public struct CursorSweepKey
    {
        public Vector2 keyFrame;
        public CursorSweepData cursorSweepData;
    }
    [Serializable]
    public struct UniqueMoveSpeedKey
    {
        public Vector2 keyFrame;
        public float moveSpeed;
    }
    [Serializable]
    public struct UniqueDragKey
    {
        public Vector2 keyFrame;
        public float drag;
    }
    [Serializable]
    public struct UniqueActionKey
    {
        public Vector2 keyFrame;
        public string uniqueActName;
    }
    [Serializable]
    public struct AnimationKey
    {
        public Vector2 keyFrame;
        public string animationName;
        public bool useOriginalAnimTime;
    }
    [Serializable]
    public struct Data
    {
        public string name;
        public int totalFrame;
        public TriggerInputType triggerInputType;
        public Vector2[] attackMotionLockKeys;
        public AttackKey[] meleeAttackKeys;
        public AttackKey[] shotKeys;
        public SummonKey[] summonKeys;
        public MoveKey[] moveKeys;
        public ImpulseMoveKey[] impulseMoveKeys;
        public CursorSweepKey[] cursorSweepKeys;
        public Vector2[] directionLockKeys;
        public UniqueMoveSpeedKey[] uniqueMoveSpeedKeys;
        public UniqueDragKey[] uniqueDragKeys;
        public UniqueActionKey[] uniqueActionKeys;
        public Vector2[] shieldKeys;
        public Vector2[] disAllowGroundSetKeys;
        public CharaData.SEKey[] seKeys;
        public CharaData.UniqueMotionStateKey[] uniqueMotionStateKeys;
        public AnimationKey[] animationKeys;
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