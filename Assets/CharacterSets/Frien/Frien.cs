//?¿½t?¿½?¿½?¿½?¿½
public class Frien : Enemy
{
    const float moveSpeed = 1;

    bool awake;

    protected override void CharaUpdate()
    {
        //?¿½Ü‚ï¿½?¿½?¿½?¿½Í‚É‚ï¿½?¿½éŒ©?¿½?¿½?¿½?¿½?¿½T?¿½[?¿½`
        LiveEntity[] friends = GetFriends();
        //?¿½T?¿½[?¿½`?¿½?¿½?¿½?¿½?¿½?¿½?¿½?¿½?¿½Ì“ï¿½?¿½?¿½Ì‚Å‚ï¿½?¿½?¿½?¿½?¿½Å‚ï¿½?¿½?¿½?¿½?¿½Ú‚ï¿½?¿½o?¿½Ü‚ï¿½
        for (int i = 0; i < friends.Length; i++)
        {
            if (!friends[i].IsLive())
            {
                awake = true;
                break;
            }
        }

        //?¿½Ú‚ï¿½?¿½o?¿½Ü‚ï¿½?¿½Ä‚ï¿½?¿½éŽ?
        if (awake)
        {
            //?¿½ß‚ï¿½?¿½É“G?¿½?¿½?¿½?¿½?¿½?¿½?¿½?¿½Ú‹ï¿½
            if (GetNearestTarget() != null)
            {
                targetCursor = transform.InverseTransformPoint(
                GetNearestTarget().transform.position).normalized;
                GetLiveEntity().SetMovement(GetLiveEntity().GetMovement() + targetCursor * moveSpeed);
            }
            //?¿½?¿½ÉU?¿½?¿½
            if (!IsAttacking())
            {
                SetAttackMotion(GetData().GetDefaultAttackMotionName());
            }
        }
    }
}