using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�L�����t
public class Cub_E : Enemy
{
    Vector3 targetCursor;

    private void Start()
    {
        maxHP = 10;
    }

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
            if (GetAttackProgress() < 0.5f)
            {
                //�W�I�̏�Ɉړ�
                Vector3 target = targetCursor
                    + transform.TransformPoint(new Vector3(0, 3, 0))
                    - transform.position;
                movement = transform.InverseTransformPoint(target)
                    / Mathf.Deg2Rad * 0.1f;
                //���̊Ԃ͒n�`�ɐG��Ă��������ɑ��������Ȃ�
                DisAllowGroundSet();
            }
        }
        else
        {
            //�U�����쒆�łȂ����Ɋl������������U�������
            if (GetNearestTarget() != null)
            {
                targetCursor = GetNearestTarget().transform.position;
                SetAttackMotion("upperAim", 120);
            }
        }
    }
}