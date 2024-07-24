using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField]
    BossBehaviourData behaviourData;
    int behaviourProgress;
    LiveEntity target;
    protected Vector3 targetPos;
    protected override void LiveEntityUpdate()
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
            targetPos = target.transform.position;
        }
    }
    protected void TargetChange()
    {
        LiveEntity[] liveEntities = GetTargets();
        if (liveEntities.Length > 0)
        {
            target = liveEntities[Random.Range(0, liveEntities.Length - 1)];
        }
    }
}
