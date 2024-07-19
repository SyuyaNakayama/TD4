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

    bool dataLock;

    protected override void ItemUpdate()
    {
        dataLock = true;

        //見た目をキャラに対応させる
        chip.material.SetColor("_AddColor", data.GetThemeColor());
        label.sprite = data.GetIconGraph();
    }

    protected override void ItemActivation(Player player)
    {
        player.EquipCharacter(data);
    }

    public void SetData(CharaData setData)
    {
        if (!dataLock)
        {
            data = setData;
            dataLock = true;
        }
    }
}