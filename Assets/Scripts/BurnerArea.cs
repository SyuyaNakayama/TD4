using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnerArea : MonoBehaviour
{
    const int maxBurnFrame = 300;

    [SerializeField]
    Texture disabledTex;
    [SerializeField]
    Texture touchingTex;
    [SerializeField]
    Texture burnTex;
    [SerializeField]
    AttackArea attackArea;
    [SerializeField]
    ScreenPasteManager spManager;

    int burnFrame;
    bool touching;
    bool prevTouching;

    void FixedUpdate()
    {
        //いったん触れて離れたら着火
        if(!IsBurning() && !touching && prevTouching)
        {
            burnFrame = maxBurnFrame;
        }
        attackArea.gameObject.SetActive(IsBurning());

        Renderer renderer = GetComponent<Renderer>();
        //状態に応じて見た目を変える
        if(IsBurning())
        {
            renderer.materials[0].SetTexture("_MainTex", burnTex);
        }
        else if(touching)
        {
            renderer.materials[0].SetTexture("_MainTex", touchingTex);
        }
        else
        {
            renderer.materials[0].SetTexture("_MainTex", disabledTex);
        }

        prevTouching = touching;
        touching = false;
        burnFrame--;

        Vector2 offset = new Vector2(0, 0);
        if(IsBurning())
        {
            offset = new Vector2(0, Mathf.Repeat(-Time.time * 4, 1));
        }
        spManager.offset = offset;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.GetComponent<LiveEntity>() != null)
        {
            touching = true;
        }
    }

    bool IsBurning()
    {
        return burnFrame > 0;
    }
}
