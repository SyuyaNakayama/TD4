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
    string teamID;
    protected override void GGOUpdate()
    {
        Vector2 movementXZ = new Vector2(movement.x, movement.z);
        movementXZ = movementXZ.normalized * speed;
        movement = new Vector3(movementXZ.x, movement.y, movementXZ.y);

        billBoard.rotAngle += speed * rotSpeed;
        speed *= speedDiffuseIntensity;

        attackArea.gameObject.SetActive(speed >= minAttackSpeed);
    }

    protected override void GGOOnCollisionStay(Collision col)
    {
        if (speed < minAttackSpeed)
        {
            if (col.gameObject.GetComponent<LiveEntity>() != null)
            {
                Shoot(col.transform.position, col.gameObject.GetComponent<LiveEntity>().GetTeamID());
            }
            if (col.gameObject.GetComponent<Spinner>() != null
            && col.gameObject.GetComponent<Spinner>().speed >= minAttackSpeed)
            {
                Shoot(col.transform.position, col.gameObject.GetComponent<Spinner>().teamID);
            }
        }
    }

    void Shoot(Vector3 shooterWorldPos, string setTeamID)
    {
        movement =
            -transform.InverseTransformPoint(shooterWorldPos)
            .normalized;
        speed = maxSpeed;
        teamID = setTeamID;

        audioSource.Play();
    }
}
