using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LiveEntity
{
    [SerializeField]
    Sensor sensor;
    protected override void LiveEntityUpdate()
    {
        //�����Ŋe�h���N���X�̌ŗL�X�V�������Ă�
        EnemyUpdate();
    }

    //�e�h���N���X�̌ŗL�X�V�����i�h���N���X���ŃI�[�o�[���C�h���Ďg���j
    protected virtual void EnemyUpdate()
    {
    }
}