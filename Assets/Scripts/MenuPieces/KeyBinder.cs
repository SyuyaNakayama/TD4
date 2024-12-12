using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField]
    KeyCode[] bindKeys = { };
    public KeyCode[] GetBindKeys()
    {
        return KX_netUtil.CopyArray<KeyCode>(bindKeys);
    }

    KeyCode[] pressingKeys = { };
    KeyCode[] prevPressingKeys = { };
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
        prevPressingKeys = KX_netUtil.CopyArray<KeyCode>(pressingKeys);
        //���݉����Ă���L�[�̔z������Z�b�g
        Array.Resize(ref pressingKeys, 0);

        //�����L�[�������ꂽ��
        if (Input.anyKey)
        {
            //�ǂ̃L�[�������ꂽ�̂����ׂ�
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(code))
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

        bindKeysName.text = "";
        for (int i = 0; i < bindKeys.Length; i++)
        {
            bindKeysName.text += bindKeys[i].ToString() + "  ";
        }
    }
}
