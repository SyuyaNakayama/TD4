using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSwitch : Switch
{
    const int maxPushCoolTimeFrame = 2;

    [SerializeField]
    Sprite off;
    [SerializeField]
    Sprite[] countDown = { };
    [SerializeField]
    int timeFrame;
    float timeAmount;

    void FixedUpdate()
    {
        if (pushCoolTimeFrame > 0)
        {
            GetComponent<SpriteRenderer>().sprite = pushed;
        }
        else if (active)
        {
            GetComponent<SpriteRenderer>().sprite =
                countDown[Mathf.Clamp(
                    Mathf.RoundToInt((1 - timeAmount) * countDown.Length - 0.5f)
                    , 0, countDown.Length - 1)];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = off;
        }
        pushCoolTimeFrame = Mathf.Clamp(pushCoolTimeFrame - 1, 0, maxPushCoolTimeFrame);

        active = timeAmount > 0;
        timeAmount = Mathf.Clamp(timeAmount - 1f / Mathf.Max(timeFrame, 1), 0, 1);
    }

    void OnTriggerStay(Collider col)
    {
        if (col.GetComponent<LiveEntity>() != null)
        {
            if (pushCoolTimeFrame <= 0)
            {
                GetComponent<AudioSource>().Play();
            }
            pushCoolTimeFrame = maxPushCoolTimeFrame;
            timeAmount = 1;
        }
    }
}
