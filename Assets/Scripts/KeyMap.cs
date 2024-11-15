using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateKeyMap")]
public class KeyMap : ScriptableObject
{
    [System.Serializable]
    //TODO:配列を使っているので外部から書き換えられないようにする
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
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
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
