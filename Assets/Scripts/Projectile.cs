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
        //内部変数が変更されないようにロック
        projectileDataLock = true;

        //必要に応じて地面に張り付くようにする
        ggobj.SetAllowGroundSet(projectileData.setGround);
        solidCollider.enabled = projectileData.setGround;

        //回転角とスケーリングを考慮した方向、スピードで飛んでいく
        ggobj.SetMovement(moveVec * projectileData.speed / Time.deltaTime);

        //visualRigを移動方向に向ける
        GetVisurlRig().localRotation = Quaternion.LookRotation(moveVec);
        GetVisurlRig().Rotate(new Vector3(-90, 0, 0), Space.Self);

        //寿命が尽きたら消える
        projectileData.lifetime--;
        if (projectileData.lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetProjectileData(AttackMotionData.ProjectileData setData, Vector3 setMoveVec)
    {
        //生成された直後のみ実行
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