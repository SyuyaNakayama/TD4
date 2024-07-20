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

    //�e�h���N���X�̌ŗL�X�V�����i�h���N���X���ŃI�[�o�[���C�h���Ďg���j
    protected virtual void AttackAreaUpdate()
    {
    }

    public void Lock()
    {
        //�������ꂽ����̂ݎ��s
        if (isNewborn)
        {
            dataLock = true;
        }
    }

    public void SetAttacker(LiveEntity setAttacker)
    {
        //�M�~�b�N�̍U������ȂǁA���b�N����Ă��Ȃ��ꍇ�̂ݎ��s
        if (!dataLock)
        {
            attacker = setAttacker;
        }
    }
    public void SetData(AttackMotionData.AttackData setData, Vector3 setBlowVec)
    {
        //���b�N����Ă��Ȃ�����Attacker��FixedUpdate()���ŌĂ΂ꂽ�ꍇ�̂ݎ��s
        if (!dataLock || attacker != null && attacker.GetUpdating())
        {
            data = setData;
            blowVec = setBlowVec;
        }
    }
}