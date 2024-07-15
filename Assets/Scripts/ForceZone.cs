using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceZone : MonoBehaviour
{
    [SerializeField]
    float force;
    public float GetForce()
    {
        return force;
    }

    void OnCollisionStay(Collision col)
    {
        OnHit(col.collider);
    }
    void OnTriggerStay(Collider col)
    {
        OnHit(col);
    }
    void OnHit(Collider col)
    {
        GeoGroObject targetObj =
            col.gameObject.GetComponent<GeoGroObject>();
        if (targetObj != null)
        {
            targetObj.AddFieldMove(
                transform.rotation * new Vector3(0, force, 0));
        }
    }
}