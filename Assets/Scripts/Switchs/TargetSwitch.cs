using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSwitch : Switch
{
    [SerializeField]
    Block block;

    void FixedUpdate()
    {
        active = block.GetBreaked();
    }
}
