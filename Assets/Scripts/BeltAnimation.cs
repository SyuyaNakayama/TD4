using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltAnimation : MonoBehaviour
{
    [SerializeField]
    ForceZone belt;
    float speed;
    void OnWillRenderObject()
    {
        if (belt != null)
        {
            speed = belt.GetForce() / belt.gameObject.transform.localScale.y;
        }
        float x = Mathf.Repeat(Time.time * speed * transform.lossyScale.y / transform.lossyScale.x, 1);
        Vector2 offset = new Vector2(0, x * -1);
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
        GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, transform.lossyScale.y / transform.lossyScale.x);

    }
}