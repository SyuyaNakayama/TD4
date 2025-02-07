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
    KX_netUtil.XInputButton[] bindButtons = { };
    public KX_netUtil.XInputButton[] GetBindButtons()
    {
        return KX_netUtil.CopyArray<KX_netUtil.XInputButton>(bindButtons);
    }
    KX_netUtil.XInputButton[] pressingButtons = { };
    KX_netUtil.XInputButton[] prevPressingButtons = { };
    bool done;

    protected override void MenuUpdate()
    {
        //�J�����u�ԃ��Z�b�g
        if (!GetPrevIsCurrentMenu())
        {
            Array.Resize(ref bindKeys, 0);
            Array.Resize(ref bindButtons, 0);
            done = false;
        }

        //�O�t���[���ŉ����Ă����L�[�ƃ{�^�����L�^
        prevPressingKeys = KX_netUtil.CopyArray<Key>(pressingKeys);
        prevPressingButtons = KX_netUtil.CopyArray<KX_netUtil.XInputButton>(pressingButtons);
        //���݉����Ă���L�[�ƃ{�^���̔z������Z�b�g
        Array.Resize(ref pressingKeys, 0);
        Array.Resize(ref pressingButtons, 0);

        //�����{�^���������ꂽ��
        if (Gamepad.all.ToArray().Length > 0
            && KX_netUtil.ISAnyPadButton(0))
        {
            //�ǂ̃{�^���������ꂽ�̂����ׂ�
            foreach (KX_netUtil.XInputButton button in Enum.GetValues(typeof(KX_netUtil.XInputButton)))
            {
                if (KX_netUtil.GetISPadButton(0, button))
                {
                    //���݉����Ă���L�[�̔z��ɒǉ�
                    Array.Resize(ref pressingButtons, pressingButtons.Length + 1);
                    pressingButtons[pressingButtons.Length - 1] = button;

                    //���̃L�[�̉����n�߂����ׂ�
                    bool isButtonDown = true;
                    for (int i = 0; i < prevPressingButtons.Length; i++)
                    {
                        if (prevPressingButtons[i] == button)
                        {
                            isButtonDown = false;
                            break;
                        }
                    }
                    //���̃L�[�̉����n�߂Ȃ�
                    if (isButtonDown)
                    {
                        //���ɐݒ肵���L�[�����ׂ�
                        bool isBindedButton = false;
                        for (int i = 0; i < bindButtons.Length; i++)
                        {
                            if (bindButtons[i] == button)
                            {
                                isBindedButton = true;
                                break;
                            }
                        }
                        //���ɐݒ肵���L�[�Ȃ�
                        if (isBindedButton)
                        {
                            //�ݒ��ʂ���鏀��
                            done = true;
                        }
                        else
                        {
                            //�ݒ肷��L�[�̔z��ɒǉ�
                            Array.Resize(ref bindButtons, bindButtons.Length + 1);
                            bindButtons[bindButtons.Length - 1] = button;
                        }
                    }
                }
            }
        }

        //�����L�[�������ꂽ��
        if (KX_netUtil.ISAnyKey())
        {
            //�ǂ̃L�[�������ꂽ�̂����ׂ�
            foreach (Key code in Enum.GetValues(typeof(Key)))
            {
                if (KX_netUtil.GetISKey(code))
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

        if (!KX_netUtil.ISAnyKey()
            && (Gamepad.all.ToArray().Length > 0
            && !KX_netUtil.ISAnyPadButton(0))
            && done)
        {
            //�L�[�o�C���h��K�p���A�ݒ��ʂ����
            manager.ApplyKeyBind();
            active = false;
        }

        //���͂����L�[��\��
        bindKeysName.text = "";

        if (bindKeys.Length > 0)
        {
            bindKeysName.text += "[Keyboard]\n";
            for (int i = 0; i < bindKeys.Length; i++)
            {
                bindKeysName.text += bindKeys[i].ToString() + "  ";
            }
            bindKeysName.text += "\n";
        }

        if (bindButtons.Length > 0)
        {
            bindKeysName.text += "[Gamepad]\n";
            for (int i = 0; i < bindButtons.Length; i++)
            {
                bindKeysName.text += bindButtons[i].ToString() + "  ";
            }
        }
    }
}