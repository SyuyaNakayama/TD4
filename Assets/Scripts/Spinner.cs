using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : GeoGroObject
{
    const float maxSpeed = 20;
    const float rotSpeed = 3;
    const float speedDiffuseIntensity = 0.995f;

    [SerializeField]
    BillBoard billBoard;
    float speed;
    protected override void GGOUpdate()
    {
        movement = movement.normalized * speed;
        billBoard.rotAngle += speed * rotSpeed;
        speed *= speedDiffuseIntensity;
    }

    protected override void GGOOnCollisionStay(Collision col)
    {
        if (col.gameObject.GetComponent<LiveEntity>() != null)
        {
            movement =
                -transform.InverseTransformPoint(col.transform.position)
                .normalized;
            speed = maxSpeed;
        }
    }
}
