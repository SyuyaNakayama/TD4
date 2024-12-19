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
        //�������O�̂��̂�T��
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                //��ł�������Ă�����true��Ԃ�
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
    //TODO:��肩���Ȃ̂Ŋ���������
    public bool GetButton(int playerIndex, string name)
    {
        //�������O�̂��̂�T��
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                //��ł�������Ă�����true��Ԃ�
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
        //�������O�̂��̂��������炻�̒��g�𕡐����ĕԂ�
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                return KX_netUtil.CopyArray<Key>(keyMapCells[i].keys);
            }
        }
        //������΋�̔z���Ԃ�
        return new Key[] { };
    }

    public void SetKeyMap(string name, Key[] keys)
    {
        //�������O�̂��̂���������㏑��
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                keyMapCells[i].keys = KX_netUtil.CopyArray<Key>(keys);
                return;
            }
        }
        //������ΐV�K�쐬
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
        //�������O�̂��̂��������炻���Ԃ�
        for (int i = 0; i < vectorInputMapCells.Length; i++)
        {
            if (vectorInputMapCells[i].name == name)
            {
                return vectorInputMapCells[i].axisBindData;
            }
        }
        //������΋�̗v�f��Ԃ�
        return new AxisBindData();
    }

    public void SetVectorInputMap(string name, AxisBindData axisBindData)
    {
        //�������O�̂��̂���������㏑��
        for (int i = 0; i < vectorInputMapCells.Length; i++)
        {
            if (vectorInputMapCells[i].name == name)
            {
                vectorInputMapCells[i].axisBindData = axisBindData;
                return;
            }
        }
        //������ΐV�K�쐬
        Array.Resize(ref vectorInputMapCells, vectorInputMapCells.Length + 1);
        vectorInputMapCells[vectorInputMapCells.Length - 1].name = name;
        vectorInputMapCells[vectorInputMapCells.Length - 1].axisBindData =
            axisBindData;
    }
}
