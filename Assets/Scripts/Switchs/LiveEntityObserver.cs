using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveEntityObserver : Switch
{
    [SerializeField]
    Sprite trueImg;
    [SerializeField]
    Sprite falseImg;
    [SerializeField]
    LiveEntity[] liveEntities = { };
    [SerializeField]
    BattleField[] battleFields = { };

    void FixedUpdate()
    {
        active = false;

        for (int i = 0; i < liveEntities.Length; i++)
        {
            if (liveEntities[i] != null && liveEntities[i].IsLive())
            {
                active = true;
                break;
            }
        }

        for (int i = 0; i < battleFields.Length; i++)
        {
            if (battleFields[i] != null && !battleFields[i].GetBattled())
            {
                active = true;
                break;
            }
        }

        if (active)
        {
            GetComponent<SpriteRenderer>().sprite = trueImg;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = falseImg;
        }

    }
}
