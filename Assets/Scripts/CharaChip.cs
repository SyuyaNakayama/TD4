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

    protected override void ItemUpdate()
    {
        chip.material.SetColor("_AddColor", data.GetThemeColor());
        label.sprite = data.GetIconGraph();
    }
}
