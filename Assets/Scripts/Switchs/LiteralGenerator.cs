using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiteralGenerator : Switch
{
    [SerializeField]
    Sprite trueImg;
    [SerializeField]
    Sprite falseImg;
    [SerializeField]
    bool value;

    void FixedUpdate()
    {
        if (value)
        {
            GetVisual().sprite = trueImg;
        }
        else
        {
            GetVisual().sprite = falseImg;
        }

        active = value;
    }
}
