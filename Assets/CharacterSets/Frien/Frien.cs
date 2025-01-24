//?��t?��?��?��?��
public class Frien : Enemy
{
    const float moveSpeed = 1;

    bool awake;

    protected override void CharaUpdate()
    {
        //?��܂�?��?��?��͂ɂ�?��錩?��?��?��?��?��T?��[?��`
        LiveEntity[] friends = GetFriends();
        //?��T?��[?��`?��?��?��?��?��?��?��?��?��̓�?��?��̂ł�?��?��?��?��ł�?��?��?��?��ڂ�?��o?��܂�
        for (int i = 0; i < friends.Length; i++)
        {
            if (!friends[i].IsLive())
            {
                awake = true;
                break;
            }
        }

        //?��ڂ�?��o?��܂�?��Ă�?���?
        if (awake)
        {
            //?��߂�?��ɓG?��?��?��?��?��?��?��?��ڋ�
            if (GetNearestTarget() != null)
            {
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
                GetLiveEntity().SetMovement(GetLiveEntity().GetMovement() + targetCursor * moveSpeed);
            }
            //?��?��ɍU?��?��
            if (!IsAttacking())
            {
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}