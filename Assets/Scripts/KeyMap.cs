using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "CreateKeyMap")]
public class KeyMap : ScriptableObject
{
    [System.Serializable]
    public struct KeyMapCell
    {
        public string name;
        public Key[] keys;
        public string[] buttons;

    }
    [System.Serializable]
    public struct VectorInputMapCell
    {
        public string name;
        public AxisBindData axisBindData;
    }
    [System.Serializable]
    public struct AxisBindData
    {
        public string axisNameX;
        public bool inverseX;
        public string axisNameY;
        public bool inverseY;
    }

    [SerializeField]
    string name;
    [SerializeField]
    KeyMapCell[] keyMapCells = { };
    [SerializeField]
    VectorInputMapCell[] vectorInputMapCells = { };

    public bool GetKey(string name)
    {
        //同じ名前のものを探す
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                //一つでも押されていたらtrueを返す
                for (int j = 0; j < keyMapCells[i].keys.Length; j++)
                {
                    if (KX_netUtil.GetIMKey(keyMapCells[i].keys[j]))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    //TODO:作りかけなので完成させる
    public bool GetButton(int playerIndex, string name)
    {
        //同じ名前のものを探す
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                //一つでも押されていたらtrueを返す
                for (int j = 0; j < keyMapCells[i].keys.Length; j++)
                {
                    if (KX_netUtil.GetIMKey(keyMapCells[i].keys[j]))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public Key[] GetKeyMap(string name)
    {
        //同じ名前のものがあったらその中身を複製して返す
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                return KX_netUtil.CopyArray<Key>(keyMapCells[i].keys);
            }
        }
        //無ければ空の配列を返す
        return new Key[] { };
    }

    public void SetKeyMap(string name, Key[] keys)
    {
        //同じ名前のものがあったら上書き
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                keyMapCells[i].keys = KX_netUtil.CopyArray<Key>(keys);
                return;
            }
        }
        //無ければ新規作成
        Array.Resize(ref keyMapCells, keyMapCells.Length + 1);
        keyMapCells[keyMapCells.Length - 1].name = name;
        keyMapCells[keyMapCells.Length - 1].keys =
            KX_netUtil.CopyArray<Key>(keys);
    }

    public Vector2 GetVectorInput(string name)
    {
        for (int i = 0; i < vectorInputMapCells.Length; i++)
        {
            /*if (vectorInputMapCells[i].name == name)
            {
                Vector2 ret = new Vector2(
                    Input.GetAxis(vectorInputMapCells[i].axisBindData.axisNameX),
                    Input.GetAxis(vectorInputMapCells[i].axisBindData.axisNameY)
                );
                return ret;
            }*/
        }
        return Vector2.zero;
    }

    public AxisBindData GetVectorInputMap(string name)
    {
        //同じ名前のものがあったらそれを返す
        for (int i = 0; i < vectorInputMapCells.Length; i++)
        {
            if (vectorInputMapCells[i].name == name)
            {
                return vectorInputMapCells[i].axisBindData;
            }
        }
        //無ければ空の要素を返す
        return new AxisBindData();
    }

    public void SetVectorInputMap(string name, AxisBindData axisBindData)
    {
        //同じ名前のものがあったら上書き
        for (int i = 0; i < vectorInputMapCells.Length; i++)
        {
            if (vectorInputMapCells[i].name == name)
            {
                vectorInputMapCells[i].axisBindData = axisBindData;
                return;
            }
        }
        //無ければ新規作成
        Array.Resize(ref vectorInputMapCells, vectorInputMapCells.Length + 1);
        vectorInputMapCells[vectorInputMapCells.Length - 1].name = name;
        vectorInputMapCells[vectorInputMapCells.Length - 1].axisBindData =
            axisBindData;
    }
}
