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
        //開いた瞬間リセット
        if (!GetPrevIsCurrentMenu())
        {
            Array.Resize(ref bindKeys, 0);
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
                        bool isBindedKey = false;
                        for (int i = 0; i < bindKeys.Length; i++)
                        {
                            if (bindKeys[i] == code)
                            {
                                isBindedKey = true;
                                break;
                            }
                        }
                        //既に設定したキーなら
                        if (isBindedKey)
                        {
                            //設定画面を閉じる準備
                            done = true;
                        }
                        else
                        {
                            //設定するキーの配列に追加
                            Array.Resize(ref bindKeys, bindKeys.Length + 1);
                            bindKeys[bindKeys.Length - 1] = code;
                        }
                    }
                }
            }
        }
        else if (done)
        {
            //キーバインドを適用し、設定画面を閉じる
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
