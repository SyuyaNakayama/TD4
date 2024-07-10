using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class BillBoard : MonoBehaviour
{
    public bool yBill;
    public bool xBill;
    public bool turnY;
    public bool UI;
    public float rotAngle;
    void OnWillRenderObject()
    {
        if (UI)
        {
            transform.rotation = Camera.current.transform.rotation;
        }
        else
        {
            transform.LookAt(Camera.current.transform.position);
        }
        if (yBill)
        {
            if (xBill)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
                if (turnY && transform.localEulerAngles.y > 90 && transform.localEulerAngles.y < 270)
                {
                    transform.Rotate(0, 180, 0, Space.Self);
                }
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
                if (turnY && transform.localEulerAngles.y > 90 && transform.localEulerAngles.y < 270)
                {
                    transform.Rotate(0, 180, 0, Space.Self);
                }
            }
        }

        transform.Rotate(0, 0, rotAngle, Space.Self);
    }
}
