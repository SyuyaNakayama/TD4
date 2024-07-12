using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : GeoGroObject
{
    const int maxInvincibleFrame = 120;
    const float popUpHeight = 3;

    [SerializeField]
    Transform visual;
    int invincibleFrame;

    protected override void GGOAwake()
    {
        invincibleFrame = maxInvincibleFrame;
    }
    protected override void GGOUpdate()
    {
        ItemUpdate();

        invincibleFrame = Mathf.Max(0, invincibleFrame - 1);
        visual.transform.localPosition = new Vector3(0,
            Mathf.Sin((float)invincibleFrame / maxInvincibleFrame * Mathf.PI)
            * popUpHeight,
            0);
    }

    public void Activation(Player player)
    {
        //そのプレイヤーがこのアイテムに触れていたかを確認
        //(不正にアイテムを取得するチートを防止するためこのような措置を取っています)
        if (invincibleFrame <= 0 && player != null
        && player.IsTouchedThisItem(this))
        {
            ItemActivation(player);
            Destroy(gameObject);
            return;
        }
    }

    protected virtual void ItemUpdate()
    {
    }
    protected virtual void ItemActivation(Player player)
    {
    }
}
