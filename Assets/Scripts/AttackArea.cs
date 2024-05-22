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
    bool dataLock = false;

    void FixedUpdate()
    {
        dataLock = true;
    }

    public void SetAttacker(LiveEntity setAttacker)
    {
        //生成された直後のみ実行
        if (attacker == null && !dataLock)
        {
            attacker = setAttacker;
        }
    }
    public void SetData(AttackMotionData.AttackData setData)
    {
        //AttackerのFixedUpdate()内で呼ばれた場合のみ実行
        if (attacker != null && attacker.GetUpdating())
        {
            data = setData;
        }
    }
}