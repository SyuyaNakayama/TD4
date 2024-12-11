using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicPiece : Switch
{
    const int maxPushCoolTimeFrame = 2;

    public enum LogicType
    {
        AND,
        OR,
        XOR
    }

    [SerializeField]
    Sprite nutralButtonImg;
    [SerializeField]
    Sprite on;
    [SerializeField]
    Sprite off;
    [SerializeField]
    Sprite[] logicIconImgs = { };
    [SerializeField]
    LogicType[] types = { LogicType.AND };
    [SerializeField]
    Switch[] input = { };
    [SerializeField]
    SpriteRenderer button;
    [SerializeField]
    SpriteRenderer logicIcon;

    int currentLogicTypeIndex;

    void FixedUpdate()
    {
        if (pushCoolTimeFrame > 0)
        {
            button.sprite = GetPushed();
        }
        else
        {
            button.sprite = nutralButtonImg;
        }

        active = false;
        int inputCount = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i].GetActive())
            {
                inputCount++;
            }
        }

        currentLogicTypeIndex = Mathf.RoundToInt(Mathf.Repeat(currentLogicTypeIndex, types.Length));
        logicIcon.sprite = logicIconImgs[(int)types[currentLogicTypeIndex]];

        switch (types[currentLogicTypeIndex])
        {
            case LogicType.AND:
                active = inputCount == input.Length;
                break;
            case LogicType.OR:
                active = inputCount >= 1;
                break;
            case LogicType.XOR:
                active = inputCount >= 1 && inputCount < input.Length;
                break;
        }

        if (active)
        {
            GetVisual().sprite = on;
        }
        else
        {
            GetVisual().sprite = off;
        }
        pushCoolTimeFrame = Mathf.Clamp(pushCoolTimeFrame - 1, 0, maxPushCoolTimeFrame);

        GetComponent<Collider>().enabled = button.enabled = types.Length > 1;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.GetComponent<LiveEntity>() != null)
        {
            if (pushCoolTimeFrame <= 0)
            {
                GetComponent<AudioSource>().Play();
                currentLogicTypeIndex++;
            }
            pushCoolTimeFrame = maxPushCoolTimeFrame;
        }
    }
}
