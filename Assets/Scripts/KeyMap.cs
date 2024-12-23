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
        public KX_netUtil.XInputButton[] buttons;

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
        public KX_netUtil.XInputAxis axisX;
        public bool inverseX;
        public KX_netUtil.XInputAxis axisY;
        public bool inverseY;
    }

    [SerializeField]
    string name;
    [SerializeField]
    KeyMapCell[] keyMapCells = { };
    [SerializeField]
    VectorInputMapCell[] vectorInputMapCells = { };

    public bool GetKey(int playerIndex, string name)
    {
        ControlMapManager.PlayerInputDevice player =
            ControlMapManager.GetPlayers()[playerIndex];

        //同じ名前のものを探す
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            //キーボード
            if (keyMapCells[i].name == name)
            {
                //一つでも押されていたらtrueを返す
                for (int j = 0; j < keyMapCells[i].keys.Length; j++)
                {
                    if (KX_netUtil.GetISKey(keyMapCells[i].keys[j]))
                    {
                        return true;
                    }
                }
            }
            //ゲームパッド
            if (keyMapCells[i].name == name)
            {
                //一つでも押されていたらtrueを返す
                for (int j = 0; j < keyMapCells[i].buttons.Length; j++)
                {
                    //割り当てられているすべてのコントローラーで試す
                    for (int k = 0; k < player.gamepads.Length; k++)
                    {
                        if (KX_netUtil.GetISPadButton(
                        player.gamepads[k], keyMapCells[i].buttons[j]))
                        {
                            return true;
                        }
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

    public void SetKeyMap(string name, KX_netUtil.XInputButton[] buttons)
    {
        //同じ名前のものがあったら上書き
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                keyMapCells[i].buttons = KX_netUtil.CopyArray
                    <KX_netUtil.XInputButton>(buttons);
                return;
            }
        }
        //無ければ新規作成
        Array.Resize(ref keyMapCells, keyMapCells.Length + 1);
        keyMapCells[keyMapCells.Length - 1].name = name;
        keyMapCells[keyMapCells.Length - 1].buttons =
            KX_netUtil.CopyArray<KX_netUtil.XInputButton>(buttons);
    }

    public Vector2 GetVectorInput(int playerIndex, string name)
    {
        Vector2 ret = Vector2.zero;
        ControlMapManager.PlayerInputDevice player =
            ControlMapManager.GetPlayers()[playerIndex];
        //同じ名前のものを探す
        for (int i = 0; i < vectorInputMapCells.Length; i++)
        {
            if (vectorInputMapCells[i].name == name)
            {
                //割り当てられているすべてのコントローラーの入力を合算
                for (int k = 0; k < player.gamepads.Length; k++)
                {
                    AxisBindData currentData =
                        vectorInputMapCells[i].axisBindData;

                    Vector2 addVec = new Vector2(
                    KX_netUtil.GetISPadAxis(
                    player.gamepads[k], currentData.axisX),
                    KX_netUtil.GetISPadAxis(
                    player.gamepads[k], currentData.axisY));

                    if (currentData.inverseX)
                    {
                        addVec.x *= -1;
                    }
                    if (currentData.inverseY)
                    {
                        addVec.y *= -1;
                    }

                    ret += addVec;
                }
            }
        }
        return ret;
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
