//?申t?申?申?申?申
public class Frien : Enemy
{
    const float moveSpeed = 1;

    bool awake;

    protected override void CharaUpdate()
    {
        //?申��鐃�?申?申?申����鐃�?申���?申?申?申?申?申T?申[?申`
        LiveEntity[] friends = GetFriends();
        //?申T?申[?申`?申?申?申?申?申?申?申?申?申��鐃�?申?申����鐃�?申?申?申?申��鐃�?申?申?申?申��鐃�?申o?申��鐃�
        for (int i = 0; i < friends.Length; i++)
        {
            if (!friends[i].IsLive())
            {
                awake = true;
                break;
            }
        }

        //?申��鐃�?申o?申��鐃�?申��鐃�?申��?
        if (awake)
        {
            //?申��鐃�?申��G?申?申?申?申?申?申?申?申��鐃�
            if (GetNearestTarget() != null)
            {
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
                GetLiveEntity().SetMovement(GetLiveEntity().GetMovement() + targetCursor * moveSpeed);
            }
            //?申?申��U?申?申
            if (!IsAttacking())
            {
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}