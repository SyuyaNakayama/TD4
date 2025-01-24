using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateControlIconAtlas")]
public class ControlIconAtlas : ScriptableObject
{
    [Serializable]
    public struct BindDataCell
    {
        public string inputCode;
        public string iconName;
    }

    [SerializeField]
    BindDataCell[] buttonBindData = { };
    public BindDataCell SearchButtonBindDataCellCode(string code)
    {
        //同じ名前のものがあったらそれを返す
        for (int i = 0; i < buttonBindData.Length; i++)
        {
            if (buttonBindData[i].inputCode == code)
            {
                return buttonBindData[i];
            }
        }
        //無ければ空の要素を返す
        return new BindDataCell();
    }
    public BindDataCell SearchButtonBindDataCellName(string name)
    {
        //同じ名前のものがあったらそれを返す
        for (int i = 0; i < buttonBindData.Length; i++)
        {
            if (buttonBindData[i].iconName == name)
            {
                return buttonBindData[i];
            }
        }
        //無ければ空の要素を返す
        return new BindDataCell();
    }

    [SerializeField]
    BindDataCell[] directionBindData = { };
    public BindDataCell SearchDirectionBindDataCellCode(string code)
    {
        //同じ名前のものがあったらそれを返す
        for (int i = 0; i < directionBindData.Length; i++)
        {
            if (directionBindData[i].inputCode == code)
            {
                return directionBindData[i];
            }
        }
        //無ければ空の要素を返す
        return new BindDataCell();
    }
    public BindDataCell SearchDirectionBindDataCellName(string name)
    {
        //同じ名前のものがあったらそれを返す
        for (int i = 0; i < directionBindData.Length; i++)
        {
            if (directionBindData[i].iconName == name)
            {
                return directionBindData[i];
            }
        }
        //無ければ空の要素を返す
        return new BindDataCell();
    }
}
