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
        //ìØÇ∂ñºëOÇÃÇ‡ÇÃÇíTÇ∑
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                //àÍÇ¬Ç≈Ç‡âüÇ≥ÇÍÇƒÇ¢ÇΩÇÁtrueÇï‘Ç∑
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

    public void SetKeyMap(string name, KeyCode[] keys)
    {
        //ìØÇ∂ñºëOÇÃÇ‡ÇÃÇ™Ç†Ç¡ÇΩÇÁè„èëÇ´
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                keyMapCells[i].keys = KX_netUtil.CopyArray<KeyCode>(keys);
                return;
            }
        }
        //ñ≥ÇØÇÍÇŒêVãKçÏê¨
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
