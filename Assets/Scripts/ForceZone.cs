using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceZone : MonoBehaviour
{
    [SerializeField]
    float force;

    void OnTriggerStay(Collider col)
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
