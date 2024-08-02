using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeoGroObject))]
public class Projectile : AttackArea
{
    AttackMotionData.ProjectileData projectileData;
    bool projectileDataLock = false;
    [SerializeField]
    Vector3 moveVec;
    [SerializeField]
    GeoGroObject ggobj;
    [SerializeField]
    SpriteRenderer visual;
    protected override void AttackAreaUpdate()
    {
        //�����ϐ����ύX����Ȃ��悤�Ƀ��b�N
        projectileDataLock = true;

        //�ݒ肳�ꂽ�摜��K�p
        visual.sprite = projectileData.sprite;

        //�X�P�[�����O�̊�ƂȂ�I�u�W�F�N�g�����߂�
        GameObject scalingTarget = gameObject;
        if (GetAttacker() != null)
        {
            scalingTarget = GetAttacker().gameObject;
        }
        //��]�p�ƃX�P�[�����O���l�����������A�X�s�[�h�Ŕ��ł���
        ggobj.Move(moveVec * projectileData.speed / Time.deltaTime);

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