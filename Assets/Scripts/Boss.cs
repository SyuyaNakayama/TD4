using UnityEngine;

public class Boss : Enemy
{
    [SerializeField]
    BossBehaviourData behaviourData;
    int behaviourProgress;
    LiveEntity target;
    protected override void CharaUpdate()
    {
        if (behaviourData && !IsAttacking())
        {
            behaviourProgress =
                Mathf.RoundToInt(Mathf.Repeat(
                behaviourProgress, behaviourData.GetAttackChart().Length));
            SetAttackMotion(behaviourData.GetAttackChart()[behaviourProgress]);
            behaviourProgress++;
        }

        if (target)
        {
            targetCursor = target.transform.position;
        }

        BossUpdate();
    }
    protected void TargetChange()
    {
        LiveEntity[] liveEntities = GetTargets();
        if (liveEntities.Length > 0)
        {
            target = liveEntities[Random.Range(0, liveEntities.Length - 1)];
        }
    }

    protected virtual void BossUpdate()
    {
    }
}
