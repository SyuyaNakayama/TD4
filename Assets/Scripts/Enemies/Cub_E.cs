using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�L�����t
public class Cub_E : Enemy
{
    protected override void LiveEntityUpdate()
    {
        //y���ɂ͋�C��R��������Ȃ��悤�ɐݒ�
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;
    }
}