using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//キュルフ
public class Cub_E : Enemy
{
    protected override void LiveEntityUpdate()
    {
        //y軸には空気抵抗がかからないように設定
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;
    }
}