
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipObject : MonoBehaviour
{
    const int maxTargetLifetime = 5;

    struct LiveEntityAndSpeed
    {
        public LiveEntity liveEntity;
        public float speed;
        public int lifetime;
    }
    LiveEntityAndSpeed[] hittedLiveEntities = { };
    void FixedUpdate()
    {
        for (int i = 0; i < hittedLiveEntities.Length; i++)
        {
            if (hittedLiveEntities[i].lifetime <= 0)
            {
                hittedLiveEntities[i].liveEntity = null;
            }
            hittedLiveEntities[i].lifetime--;
        }
        List<LiveEntityAndSpeed> list =
            new List<LiveEntityAndSpeed>(hittedLiveEntities);
        list.RemoveAll(where => where.liveEntity == null);
        hittedLiveEntities = list.ToArray();
    }
    void OnCollisionStay(Collision col)
    {
        OnHit(col.collider);
    }
    void OnTriggerStay(Collider col)
    {
        OnHit(col);
    }
    void OnHit(Collider col)
    {
        LiveEntity targetObj =
            col.gameObject.GetComponent<LiveEntity>();
        if (targetObj != null)
        {
            for (int i = 0; i < hittedLiveEntities.Length; i++)
            {
                LiveEntityAndSpeed current = hittedLiveEntities[i];
                if (current.liveEntity == targetObj)
                {
                    hittedLiveEntities[i].speed =
                        Mathf.Max(current.speed,
                        CalculateSpeed(current.liveEntity));
                    current.liveEntity.Move(
                        current.liveEntity.GetMovement().normalized * current.speed);
                    hittedLiveEntities[i].lifetime = maxTargetLifetime;
                    return;
                }
            }
            Array.Resize(ref hittedLiveEntities, hittedLiveEntities.Length + 1);
            hittedLiveEntities[hittedLiveEntities.Length - 1].liveEntity = targetObj;
            hittedLiveEntities[hittedLiveEntities.Length - 1].lifetime = maxTargetLifetime;
        }
    }
    float CalculateSpeed(LiveEntity liveEntity)
    {
        return Vector3.Magnitude(liveEntity.GetMovement());
    }
}
