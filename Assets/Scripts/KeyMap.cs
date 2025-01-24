using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "CreateKeyMap")]
public class KeyMap : ScriptableObject
{
    [Serializable]
    public struct KeyMapCell
    {
        public string name;
        public Key[] keys;
        public KX_netUtil.XInputButton[] buttons;

    }
    [Serializable]
    public struct VectorInputMapCell
    {
        public string name;
        public AxisBindData axisBindData;
    }
    [Serializable]
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

        //�������O�̂��̂�T��
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            //�L�[�{�[�h
            if (keyMapCells[i].name == name)
            {
                //��ł�������Ă�����true��Ԃ�
                for (int j = 0; j < keyMapCells[i].keys.Length; j++)
                {
                    if (KX_netUtil.GetISKey(keyMapCells[i].keys[j]))
                    {
                        return true;
                    }
                }
            }
            //�Q�[���p�b�h
            if (keyMapCells[i].name == name)
            {
                //��ł�������Ă�����true��Ԃ�
                for (int j = 0; j < keyMapCells[i].buttons.Length; j++)
                {
                    //���蓖�Ă��Ă��邷�ׂẴR���g���[���[�Ŏ���
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

    public Key[] GetKeys(string name)
    {
        //�������O�̂��̂��������炻�̒��g�𕡐����ĕԂ�
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                return KX_netUtil.CopyArray(keyMapCells[i].keys);
            }
        }
        //������΋�̔z���Ԃ�
        return new Key[] { };
    }
    public KX_netUtil.XInputButton[] GetButtons(string name)
    {
        //�������O�̂��̂��������炻�̒��g�𕡐����ĕԂ�
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                return KX_netUtil.CopyArray(keyMapCells[i].buttons);
            }
        }
        //������΋�̔z���Ԃ�
        return new KX_netUtil.XInputButton[] { };
    }

    public void SetKeyMap(string name, Key[] keys)
    {
        //�������O�̂��̂���������㏑��
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                keyMapCells[i].keys = KX_netUtil.CopyArray(keys);
                return;
            }
        }
        //������ΐV�K�쐬
        Array.Resize(ref keyMapCells, keyMapCells.Length + 1);
        keyMapCells[keyMapCells.Length - 1].name = name;
        keyMapCells[keyMapCells.Length - 1].keys =
            KX_netUtil.CopyArray(keys);
    }

    public void SetKeyMap(string name, KX_netUtil.XInputButton[] buttons)
    {
        //�������O�̂��̂���������㏑��
        for (int i = 0; i < keyMapCells.Length; i++)
        {
            if (keyMapCells[i].name == name)
            {
                keyMapCells[i].buttons = KX_netUtil.CopyArray(buttons);
                return;
            }
        }
        //������ΐV�K�쐬
        Array.Resize(ref keyMapCells, keyMapCells.Length + 1);
        keyMapCells[keyMapCells.Length - 1].name = name;
        keyMapCells[keyMapCells.Length - 1].buttons =
            KX_netUtil.CopyArray(buttons);
    }

    public Vector2 GetVectorInput(int playerIndex, string name)
    {
        Vector2 ret = Vector2.zero;
        ControlMapManager.PlayerInputDevice player =
            ControlMapManager.GetPlayers()[playerIndex];
        //�������O�̂��̂�T��
        for (int i = 0; i < vectorInputMapCells.Length; i++)
        {
            if (vectorInputMapCells[i].name == name)
            {
                //���蓖�Ă��Ă��邷�ׂẴR���g���[���[�̓��͂����Z
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