using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    void OnTriggerStay(Collider Hit)
    {
        LiveEntity targetObj = Hit.GetComponent<LiveEntity>();
        if (targetObj != null)
        {
            targetObj.SetNoGravity(true);
            if (KX_netUtil.IsInsidePosition(GetComponent<Collider>(), Hit.gameObject.transform.position))
            {
                targetObj.SetMovement(targetObj.GetMovement() + new Vector3(0, 0.1f, 0));
            }
            else
            {
                targetObj.SetMovement(new Vector3(
                    targetObj.GetMovement().x,
                    targetObj.GetMovement().y / 1.1f,
                    targetObj.GetMovement().z));
            }
        }
    }
    void OnTriggerEnter(Collider Hit)
    {
        LiveEntity targetObj = Hit.GetComponent<LiveEntity>();
        if (targetObj != null)
        {
            targetObj.SetNoGravity(true);
            targetObj.SetMovement(targetObj.GetMovement() / 4);
        }
    }
}
