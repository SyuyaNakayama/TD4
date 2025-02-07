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
    Collider solidCollider;

    protected override void AttackAreaUpdate()
    {
        //�����ϐ����ύX����Ȃ��悤�Ƀ��b�N
        projectileDataLock = true;

        //�K�v�ɉ����Ēn�ʂɒ���t���悤�ɂ���
        ggobj.SetAllowGroundSet(projectileData.setGround);
        solidCollider.enabled = projectileData.setGround;

        //��]�p�ƃX�P�[�����O���l�����������A�X�s�[�h�Ŕ��ł���
        ggobj.SetMovement(moveVec * projectileData.speed / Time.deltaTime);

        //visualRig���ړ������Ɍ�����
        GetVisurlRig().localRotation = Quaternion.LookRotation(moveVec);
        GetVisurlRig().Rotate(new Vector3(-90, 0, 0), Space.Self);

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