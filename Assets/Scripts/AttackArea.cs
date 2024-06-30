using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AttackArea : MonoBehaviour
{
    LiveEntity attacker;
    public LiveEntity GetAttacker()
    {
        return attacker;
    }
    [SerializeField]
    AttackMotionData.AttackData data;
    public AttackMotionData.AttackData GetData()
    {
        return data;
    }
    [SerializeField]
    Vector3 blowVec;
    public Vector3 GetBlowVec()
    {
        return blowVec;
    }
    bool dataLock = false;

    void FixedUpdate()
    {
        dataLock = true;
        AttackAreaUpdate();
    }

    //各派生クラスの固有更新処理（派生クラス内でオーバーライドして使う）
    protected virtual void AttackAreaUpdate()
    {
    }

    public void SetAttacker(LiveEntity setAttacker)
    {
        //生成された直後のみ実行
        if (attacker == null && !dataLock)
        {
            attacker = setAttacker;
        }
    }
    public void SetData(AttackMotionData.AttackData setData, Vector3 setBlowVec)
    {
        //AttackerのFixedUpdate()内で呼ばれた場合のみ実行
        if (attacker != null && attacker.GetUpdating())
        {
            data = setData;
            blowVec = setBlowVec;
        }
    }
}