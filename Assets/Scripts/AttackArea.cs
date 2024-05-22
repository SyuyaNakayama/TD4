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
        AttackAreaUpdate();
    }

    //�e�h���N���X�̌ŗL�X�V�����i�h���N���X���ŃI�[�o�[���C�h���Ďg���j
    protected virtual void AttackAreaUpdate()
    {
    }

    public void SetAttacker(LiveEntity setAttacker)
    {
        //�������ꂽ����̂ݎ��s
        if (attacker == null && !dataLock)
        {
            attacker = setAttacker;
        }
    }
    public void SetData(AttackMotionData.AttackData setData)
    {
        //Attacker��FixedUpdate()���ŌĂ΂ꂽ�ꍇ�̂ݎ��s
        if (attacker != null && attacker.GetUpdating())
        {
            data = setData;
        }
    }
}