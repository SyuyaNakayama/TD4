using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveEntity : MonoBehaviour
{
    protected Vector3 movement;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        LiveEntityUpdate();

        GetComponent<Rigidbody>().velocity = transform.rotation * movement;

        movement *= 0.8f;
        movement += new Vector3(0, -0.5f, 0);
    }

    void OnCollisionStay(Collision col)
    {
        Vector3 localClosestPoint = transform.InverseTransformPoint(
            col.collider.ClosestPoint(transform.position));
        transform.Rotate(-Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y) / Mathf.Deg2Rad, 0, 0, Space.Self);

        localClosestPoint = transform.InverseTransformPoint(
            col.collider.ClosestPoint(transform.position));
        transform.Rotate(0, 0, Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y) / Mathf.Deg2Rad, Space.Self);

    }

    protected virtual void LiveEntityUpdate()
    {

    }
}
