using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//?øΩL?øΩ?øΩ?øΩ?øΩ?øΩt
public class Cub_E : Enemy
{
    Vector3 targetCursor;

    protected override void LiveEntityUpdate()
    {
        //y?øΩ?øΩ?øΩ…ÇÕãÔøΩC?øΩ?øΩR?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ»ÇÔøΩ?øΩÊÇ§?øΩ…ê›íÔøΩ
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;
        //?øΩd?øΩÕÇÔøΩ?øΩ?øΩ?øΩﬂÇ…ê›íÔøΩ
        gravityScale = 1;

        if (IsAttacking())
        {
            if (GetAttackProgress() < 0.5f)
            {
                //?øΩW?øΩI?øΩÃèÔøΩ…à⁄ìÔøΩ
                Vector3 target = targetCursor
                    + transform.TransformPoint(new Vector3(0, 3, 0))
                    - transform.position;
                movement = transform.InverseTransformPoint(target)
                    / Mathf.Deg2Rad * 0.1f;
                //?øΩ?øΩ?øΩÃä‘ÇÕín?øΩ`?øΩ…êG?øΩ?øΩƒÇÔøΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ…ëÔøΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ»ÇÔøΩ
                DisAllowGroundSet();
            }
        }
        else
        {
            //?øΩU?øΩ?øΩ?øΩ?øΩ?øΩ?íÜ?øΩ≈Ç»ÇÔøΩ?øΩ?øΩ?øΩ…äl?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ¬ÇÔøΩ?øΩ?øΩ?øΩ?øΩU?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ?øΩ
            if (GetNearestTarget() != null)
            {
                targetCursor = GetNearestTarget().transform.position;
                SetAttackMotion("upperAim", 120);
            }
        }
    }
}