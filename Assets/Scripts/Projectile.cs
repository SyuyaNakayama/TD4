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
    Collider collider;

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

        ggobj.SetAllowGroundSet(projectileData.setGround);
        collider.enabled = projectileData.setGround;

        //��]�p�ƃX�P�[�����O���l�����������A�X�s�[�h�Ŕ��ł���
        ggobj.SetMovement(moveVec * projectileData.speed / Time.deltaTime);

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

            if (setData.setGround && GetAttacker())
            {
                ggobj.SetCurrentGround(
                    GetAttacker().GetCurrentGround());
            }

            projectileDataLock = true;
        }
    }
}