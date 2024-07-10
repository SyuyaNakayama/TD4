using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : GeoGroObject
{
    const float maxSpeed = 20;
    const float minAttackSpeed = 3;
    const float rotSpeed = 3;
    const float speedDiffuseIntensity = 0.995f;

    [SerializeField]
    BillBoard billBoard;
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AttackArea attackArea;
    float speed;
    LiveEntity shooter;
    protected override void GGOUpdate()
    {
        Vector2 movementXZ = new Vector2(movement.x, movement.z);
        movementXZ = movementXZ.normalized * speed;
        movement = new Vector3(movementXZ.x, movement.y, movementXZ.y);

        billBoard.rotAngle += speed * rotSpeed;
        speed *= speedDiffuseIntensity;

        attackArea.gameObject.SetActive(speed >= minAttackSpeed);
        attackArea.SetAttacker(shooter);
    }

    protected override void GGOOnCollisionStay(Collision col)
    {
        if (speed < minAttackSpeed)
        {
            if (col.gameObject.GetComponent<LiveEntity>() != null)
            {
                Shoot(col.transform.position, col.gameObject.GetComponent<LiveEntity>(),
                    maxSpeed);
            }
            if (col.gameObject.GetComponent<Spinner>() != null
            && col.gameObject.GetComponent<Spinner>().speed >= minAttackSpeed)
            {
                Shoot(col.transform.position, col.gameObject.GetComponent<Spinner>().shooter,
                    col.gameObject.GetComponent<Spinner>().speed);
            }
        }
    }

    void Shoot(Vector3 shooterWorldPos, LiveEntity setShooter, float setSpeed)
    {
        movement =
            -transform.InverseTransformPoint(shooterWorldPos)
            .normalized;
        speed = setSpeed;
        shooter = setShooter;

        audioSource.Play();
    }
}
