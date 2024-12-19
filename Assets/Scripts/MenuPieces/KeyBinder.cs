using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class KeyBinder : Menu
{
    [SerializeField]
    ControlMapManager manager;
    [SerializeField]
    TMP_Text bindKeysName;
    [SerializeField]
    string keyMapCellName;
    public string GetKeyMapCellName()
    {
        return keyMapCellName;
    }

    Key[] bindKeys = { };
    public Key[] GetBindKeys()
    {
        return KX_netUtil.CopyArray<Key>(bindKeys);
    }
    Key[] pressingKeys = { };
    Key[] prevPressingKeys = { };
    bool done;

    protected override void MenuUpdate()
    {
        //�J�����u�ԃ��Z�b�g
        if (!GetPrevIsCurrentMenu())
        {
            Array.Resize(ref bindKeys, 0);
            done = false;
        }

        //�O�t���[���ŉ����Ă����L�[���L�^
        prevPressingKeys = KX_netUtil.CopyArray<Key>(pressingKeys);
        //���݉����Ă���L�[�̔z������Z�b�g
        Array.Resize(ref pressingKeys, 0);

        Debug.Log(Gamepad.all.ToArray().Length);

        //�����{�^���������ꂽ��
        if (Gamepad.all.ToArray().Length > 0
            && KX_netUtil.IMAnyPadButton(0))
        {
            //�ǂ̃{�^���������ꂽ�̂����ׂ�
            foreach (KX_netUtil.XInputButton button in Enum.GetValues(typeof(KX_netUtil.XInputButton)))
            {
                if (KX_netUtil.GetIMPadButton(0, button))
                {
                    Debug.Log(button.ToString());
                }
            }
        }

        //�����L�[�������ꂽ��
        if (KX_netUtil.IMAnyKey())
        {
            //�ǂ̃L�[�������ꂽ�̂����ׂ�
            foreach (Key code in Enum.GetValues(typeof(Key)))
            {
                if (KX_netUtil.GetIMKey(code))
                {
                    //���݉����Ă���L�[�̔z��ɒǉ�
                    Array.Resize(ref pressingKeys, pressingKeys.Length + 1);
                    pressingKeys[pressingKeys.Length - 1] = code;

                    //���̃L�[�̉����n�߂����ׂ�
                    bool isKeyDown = true;
                    for (int i = 0; i < prevPressingKeys.Length; i++)
                    {
                        if (prevPressingKeys[i] == code)
                        {
                            isKeyDown = false;
                            break;
                        }
                    }
                    //���̃L�[�̉����n�߂Ȃ�
                    if (isKeyDown)
                    {
                        //���ɐݒ肵���L�[�����ׂ�
                        bool isBindedKey = false;
                        for (int i = 0; i < bindKeys.Length; i++)
                        {
                            if (bindKeys[i] == code)
                            {
                                isBindedKey = true;
                                break;
                            }
                        }
                        //���ɐݒ肵���L�[�Ȃ�
                        if (isBindedKey)
                        {
                            //�ݒ��ʂ���鏀��
                            done = true;
                        }
                        else
                        {
                            //�ݒ肷��L�[�̔z��ɒǉ�
                            Array.Resize(ref bindKeys, bindKeys.Length + 1);
                            bindKeys[bindKeys.Length - 1] = code;
                        }
                    }
                }
            }
        }
        else if (done)
        {
            //�L�[�o�C���h��K�p���A�ݒ��ʂ����
            manager.ApplyKeyBind();
            active = false;
        }

        //���͂����L�[��\��
        bindKeysName.text = "";
        for (int i = 0; i < bindKeys.Length; i++)
        {
            bindKeysName.text += bindKeys[i].ToString() + "  ";
        }
    }
}