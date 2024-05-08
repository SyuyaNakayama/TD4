using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//?申L?申?申?申?申?申t
public class Cub_E : Enemy
{
    Vector3 targetCursor;

    protected override void LiveEntityUpdate()
    {
        //y?申?申?申����鐃�C?申?申R?申?申?申?申?申?申?申?申��鐃�?申���?申����鐃�
        dragAxis.x = true;
        dragAxis.y = false;
        dragAxis.z = true;
        //?申d?申��鐃�?申?申?申������鐃�
        gravityScale = 1;

        if (IsAttacking())
        {
            if (GetAttackProgress() < 0.5f)
            {
                //?申W?申I?申��鐃宿���鐃�
                Vector3 target = targetCursor
                    + transform.TransformPoint(new Vector3(0, 3, 0))
                    - transform.position;
                movement = transform.InverseTransformPoint(target)
                    / Mathf.Deg2Rad * 0.1f;
                //?申?申?申������n?申`?申��G?申?申��鐃�?申?申?申?申?申?申?申��鐃�?申?申?申?申?申?申?申��鐃�
                DisAllowGroundSet();
            }
        }
        else
        {
            //?申U?申?申?申?申?申?��?申����鐃�?申?申?申��l?申?申?申?申?申?申?申��鐃�?申?申?申?申U?申?申?申?申?申?申?申
            if (GetNearestTarget() != null)
            {
                targetCursor = GetNearestTarget().transform.position;
                SetAttackMotion("upperAim", 120);
            }
        }
    }
}