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
        //開いた瞬間リセット
        if (!GetPrevIsCurrentMenu())
        {
            Array.Resize(ref selectedKeys, 0);
            done = false;
        }

        //前フレームで押していたキーを記録
        prevPressingKeys = KX_netUtil.CopyArray<KeyCode>(pressingKeys);
        //現在押しているキーの配列をリセット
        Array.Resize(ref pressingKeys, 0);

        //何かキーが押されたら
        if (Input.anyKey)
        {
            //どのキーが押されたのか調べる
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(code))
                {
                    //現在押しているキーの配列に追加
                    Array.Resize(ref pressingKeys, pressingKeys.Length + 1);
                    pressingKeys[pressingKeys.Length - 1] = code;

                    //そのキーの押し始めか調べる
                    bool isKeyDown = true;
                    for (int i = 0; i < prevPressingKeys.Length; i++)
                    {
                        if (prevPressingKeys[i] == code)
                        {
                            isKeyDown = false;
                            break;
                        }
                    }
                    //そのキーの押し始めなら
                    if (isKeyDown)
                    {
                        //既に設定したキーか調べる
                        bool isSettedKey = false;
                        for (int i = 0; i < selectedKeys.Length; i++)
                        {
                            if (selectedKeys[i] == code)
                            {
                                isSettedKey = true;
                                break;
                            }
                        }
                        //既に設定したキーなら
                        if (isSettedKey)
                        {
                            //設定画面を閉じる準備
                            done = true;
                        }
                        else
                        {
                            //設定するキーの配列に追加
                            Array.Resize(ref selectedKeys, selectedKeys.Length + 1);
                            selectedKeys[selectedKeys.Length - 1] = code;
                        }
                    }
                }
            }
        }
        else if (done)
        {
            //設定画面を閉じる
            active = false;
        }

        bindKeysName.text = "";
        for (int i = 0; i < selectedKeys.Length; i++)
        {
            bindKeysName.text += selectedKeys[i].ToString() + "  ";
        }
    }
}
