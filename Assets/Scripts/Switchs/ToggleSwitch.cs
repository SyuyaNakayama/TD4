using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSwitch : Switch
{
    [SerializeField]
    Sprite on;
    [SerializeField]
    Sprite off;

    void FixedUpdate()
    {
        if (pushCoolTimeFrame > 0)
        {
            GetComponent<SpriteRenderer>().sprite = pushed;
        }
        else if (active)
        {
            GetComponent<SpriteRenderer>().sprite = on;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = off;
        }
        pushCoolTimeFrame = Mathf.Clamp(pushCoolTimeFrame - 1, 0, maxPushCoolTimeFrame);
    }

    void OnTriggerEnter(Collider col)
    {
        if (pushCoolTimeFrame <= 0 && col.GetComponent<LiveEntity>() != null)
        {
            active = !active;
            GetComponent<AudioSource>().Play();
            pushCoolTimeFrame = maxPushCoolTimeFrame;
        }
    }
}
