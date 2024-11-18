using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CharaChip : Item
{
    [SerializeField]
    SpriteRenderer chip;
    [SerializeField]
    SpriteRenderer label;
    [SerializeField]
    CharaData data;

    bool charaDataLock;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Handles.Label(
            transform.position,
            data.name);
    }
#endif

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