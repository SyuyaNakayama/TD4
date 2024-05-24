using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : AttackArea
{
    AttackMotionData.ProjectileData projectileData;
    bool projectileDataLock = false;
    Vector3 moveVec;
    protected override void AttackAreaUpdate()
    {
        //�����ϐ����ύX����Ȃ��悤�Ƀ��b�N
        projectileDataLock = true;

        //�X�P�[�����O�̊�ƂȂ�I�u�W�F�N�g�����߂�
        GameObject scalingTarget = gameObject;
        if (GetAttacker() != null)
        {
            scalingTarget = GetAttacker().gameObject;
        }
        //��]�p�ƃX�P�[�����O���l�����������A�X�s�[�h�Ŕ��ł���
        transform.position += transform.rotation * moveVec
            * projectileData.speed * scalingTarget.transform.lossyScale.x;

        //�������s�����������
        projectileData.lifetime--;
        if (projectileData.lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetProjectileData(AttackMotionData.ProjectileData setData, Vector3 setMoveVec)
    {
        //�������ꂽ����̂ݎ��s
        if (!projectileDataLock)
        {
            projectileData = setData;
            moveVec = setMoveVec;
            projectileDataLock = true;
        }
    }
}