using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaChip : Item
{
    [SerializeField]
    SpriteRenderer chip;
    [SerializeField]
    SpriteRenderer label;
    [SerializeField]
    CharaData data;

    bool charaDataLock;

    protected override void ItemUpdate()
    {
        charaDataLock = true;

        //見た目をキャラに対応させる
        chip.material.SetColor("_AddColor", data.GetThemeColor());
        label.sprite = data.GetIconGraph();
    }

    protected override void ItemActivation(LiveEntity liveEntity)
    {
        Player player = liveEntity.GetCassette().GetComponent<Player>();
        if (player)
        {
            player.EquipCharacter(data);
        }
    }

    public void SetData(CharaData setData)
    {
        if (!charaDataLock)
        {
            data = setData;
            charaDataLock = true;
        }
    }
}