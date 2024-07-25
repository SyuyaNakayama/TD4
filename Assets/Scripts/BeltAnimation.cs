using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltAnimation : MonoBehaviour
{
    [SerializeField]
    ForceZone belt;
    [SerializeField]
    int materialIndex;

    void OnWillRenderObject()
    {
        float speed = 0;
        if (belt != null)
        {
            speed = belt.GetForce() / belt.gameObject.transform.localScale.y;
        }
        float x = Mathf.Repeat(Time.time * speed * transform.lossyScale.y / transform.lossyScale.x, 1);
        Vector2 offset = new Vector2(0, x * -1);
        GetComponent<Renderer>().materials[materialIndex].SetTextureOffset("_MainTex", offset);
        GetComponent<Renderer>().materials[materialIndex].mainTextureScale = new Vector2(1, transform.lossyScale.y / transform.lossyScale.x);

    }
}