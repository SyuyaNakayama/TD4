using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyBinder : Menu
{
    [SerializeField]
    string keyMapCellName;
    [SerializeField]
    TMP_Text bindKeysName;

    KeyCode[] selectedKeys = { };
    KeyCode[] pressingKeys = { };
    KeyCode[] prevPressingKeys = { };
    bool done;

    protected override void MenuUpdate()
    {
        //�J�����u�ԃ��Z�b�g
        if (!GetPrevIsCurrentMenu())
        {
            Array.Resize(ref selectedKeys, 0);
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
                        bool isSettedKey = false;
                        for (int i = 0; i < selectedKeys.Length; i++)
                        {
                            if (selectedKeys[i] == code)
                            {
                                isSettedKey = true;
                                break;
                            }
                        }
                        //���ɐݒ肵���L�[�Ȃ�
                        if (isSettedKey)
                        {
                            //�ݒ��ʂ���鏀��
                            done = true;
                        }
                        else
                        {
                            //�ݒ肷��L�[�̔z��ɒǉ�
                            Array.Resize(ref selectedKeys, selectedKeys.Length + 1);
                            selectedKeys[selectedKeys.Length - 1] = code;
                        }
                    }
                }
            }
        }
        else if (done)
        {
            //�ݒ��ʂ����
            active = false;
        }

        bindKeysName.text = "";
        for (int i = 0; i < selectedKeys.Length; i++)
        {
            bindKeysName.text += selectedKeys[i].ToString() + "  ";
        }
    }
}
