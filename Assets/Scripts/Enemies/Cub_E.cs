using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�L�����t
public class Cub_E : Enemy
{
    Vector3 targetCursor;
    protected override void LiveEntityUpdate()
    {
        //y���ɂ͋�C��R��������Ȃ��悤�ɐݒ�
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;
        //�d�͂����߂ɐݒ�
        gravityScale = 1;

        if (IsAttacking())
        {
            if (GetAttackProgress() > 0.5f)
            {

            }
        }
        else
        {
            //�U�����쒆�łȂ����Ɋl������������U�������
            if (GetNearestTarget() != null)
            {
                targetCursor = GetNearestTarget().transform.position;
                SetAttackMotion("upperAim", 60);
            }
        }
    }
}