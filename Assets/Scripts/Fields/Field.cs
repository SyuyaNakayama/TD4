using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Field : MonoBehaviour
{
    [SerializeField]
    int fieldCode;
    Field[] willConnectField = { };
    Field[] connectedField = { };

    void FixedUpdate()
    {
        connectedField = willConnectField;
        Array.Resize(ref willConnectField, 0);
        UniqueFixedUpdate();
    }
    void OnTriggerStay(Collider col)
    {
        if (col.GetComponent<Field>() != null && col.GetComponent<Field>().fieldCode == fieldCode)
        {
            Array.Resize(ref willConnectField, willConnectField.Length + 1);
            willConnectField[willConnectField.Length - 1] = col.GetComponent<Field>();
        }
        else
        {
            pullBack(col.gameObject);
            UniqueOnTriggerStay(col);
        }
    }
    protected virtual void UniqueOnTriggerStay(Collider col)
    {
    }
    protected virtual void UniqueFixedUpdate()
    {
    }
    void OnWillRenderObject()
    {
        Vector2 tiling = new Vector2(4 * Screen.width / (float)Screen.height, 4);
        Vector2 offset = new Vector2(Mathf.Repeat(-Time.time / 8, 1), Mathf.Repeat(Time.time / 8, 1));
        GetComponent<Renderer>().materials[0].SetTextureScale("_MainTex", tiling);
        GetComponent<Renderer>().materials[0].SetTextureOffset("_MainTex", offset);
    }
    void pullBack(GameObject obj)
    {
        if (obj.GetComponent<LiveEntity>() != null)
        {
            bool needPullBack = true;
            for (int i = 0; i < connectedField.Length; i++)
            {
                if (KX_netUtil.IsInsidePosition(
                    connectedField[i].GetComponent<Collider>(),
                    obj.transform.position))
                {
                    needPullBack = false;
                    break;
                }
            }

            if (needPullBack)
            {
                obj.transform.position = GetComponent<Collider>().ClosestPoint(obj.transform.position);
            }
        }
    }
}