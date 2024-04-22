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
        
    }

    protected virtual void LiveEntityUpdate()
    {

    }
}
