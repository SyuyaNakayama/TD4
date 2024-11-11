using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�L���C��
public class Kimoime : Enemy
{
    const float stickRadius = 0.5f;
    const float stickPower = 10;

    protected override void CharaUpdate()
    {
        LiveEntity[] targets = GetTargets();
        for (int i = 0; i < targets.Length; i++)
        {
            LiveEntity target = targets[i];
            if (Vector3.Magnitude(transform.InverseTransformPoint(
                target.transform.position)) <= stickRadius)
            {
                target.SetMovement(
                    target.transform.InverseTransformPoint(transform.position)
                    * stickPower);
            }
        }
    }
}