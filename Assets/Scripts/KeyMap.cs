using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateKeyMap")]
public class KeyMap : ScriptableObject
{
    [System.Serializable]
    public struct KeyMapCell
    {
        public string name;
        public KeyCode[] keys;
    }
    [System.Serializable]
    public struct VectorInputMapCell
    {
        public string name;
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
                    if (Input.GetKey(keyMapCells[i].keys[j]))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public KeyCode[] GetKeyMap(string name)
    {
        //同じ名前のものがあったらその中身を複製して返す
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                return KX_netUtil.CopyArray<KeyCode>(keyMapCells[i].keys);
            }
        }
        //無ければ空の配列を返す
        return new KeyCode[] { };
    }

    public void SetKeyMap(string name, KeyCode[] keys)
    {
        //同じ名前のものがあったら上書き
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                keyMapCells[i].keys = KX_netUtil.CopyArray<KeyCode>(keys);
                return;
            }
        }
        //無ければ新規作成
        Array.Resize(ref keyMapCells, keyMapCells.Length + 1);
        keyMapCells[keyMapCells.Length - 1].name = name;
        keyMapCells[keyMapCells.Length - 1].keys = keys;
    }

    public Vector2 GetVectorInput(string name)
    {
        for (int i = 0; i < vectorInputMapCells.Length; i++)
        {
            if (vectorInputMapCells[i].name == name)
            {
                Vector2 ret = new Vector2(
                    Input.GetAxis(vectorInputMapCells[i].axisNameX),
                    Input.GetAxis(vectorInputMapCells[i].axisNameY)
                );
                return ret;
            }
        }
        return Vector2.zero;
    }
}
