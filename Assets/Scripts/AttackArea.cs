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
    bool isNewborn = true;
    bool dataLock = false;

    void FixedUpdate()
    {
        isNewborn = false;
        AttackAreaUpdate();
    }

    //各派生クラスの固有更新処理（派生クラス内でオーバーライドして使う）
    protected virtual void AttackAreaUpdate()
    {
    }

    public void Lock()
    {
        //生成された直後のみ実行
        if (isNewborn)
        {
            dataLock = true;
        }
    }

    public void SetAttacker(LiveEntity setAttacker)
    {
        //ギミックの攻撃判定など、ロックされていない場合のみ実行
        if (!dataLock)
        {
            attacker = setAttacker;
        }
    }
    public void SetData(AttackMotionData.AttackData setData, Vector3 setBlowVec)
    {
        //ロックされていない又はAttackerのFixedUpdate()内で呼ばれた場合のみ実行
        if (!dataLock || attacker != null && attacker.GetUpdating())
        {
            data = setData;
            blowVec = setBlowVec;
        }
    }
}