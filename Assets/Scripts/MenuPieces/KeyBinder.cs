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
        Debug.Log(Gamepad.all.Count + " " + Joystick.all.Count);

        //開いた瞬間リセット
        if (!GetPrevIsCurrentMenu())
        {
            Array.Resize(ref bindKeys, 0);
            Array.Resize(ref bindButtons, 0);
            done = false;
        }

        //前フレームで押していたキーとボタンを記録
        prevPressingKeys = KX_netUtil.CopyArray<Key>(pressingKeys);
        prevPressingButtons = KX_netUtil.CopyArray<KX_netUtil.XInputButton>(pressingButtons);
        //現在押しているキーとボタンの配列をリセット
        Array.Resize(ref pressingKeys, 0);
        Array.Resize(ref pressingButtons, 0);

        //何かボタンが押されたら
        if (Gamepad.all.ToArray().Length > 0
            && KX_netUtil.ISAnyPadButton(0))
        {
            //どのボタンが押されたのか調べる
            foreach (KX_netUtil.XInputButton button in Enum.GetValues(typeof(KX_netUtil.XInputButton)))
            {
                if (KX_netUtil.GetISPadButton(0, button))
                {
                    //現在押しているキーの配列に追加
                    Array.Resize(ref pressingButtons, pressingButtons.Length + 1);
                    pressingButtons[pressingButtons.Length - 1] = button;

                    //そのキーの押し始めか調べる
                    bool isButtonDown = true;
                    for (int i = 0; i < prevPressingButtons.Length; i++)
                    {
                        if (prevPressingButtons[i] == button)
                        {
                            isButtonDown = false;
                            break;
                        }
                    }
                    //そのキーの押し始めなら
                    if (isButtonDown)
                    {
                        //既に設定したキーか調べる
                        bool isBindedButton = false;
                        for (int i = 0; i < bindButtons.Length; i++)
                        {
                            if (bindButtons[i] == button)
                            {
                                isBindedButton = true;
                                break;
                            }
                        }
                        //既に設定したキーなら
                        if (isBindedButton)
                        {
                            //設定画面を閉じる準備
                            done = true;
                        }
                        else
                        {
                            //設定するキーの配列に追加
                            Array.Resize(ref bindButtons, bindButtons.Length + 1);
                            bindButtons[bindButtons.Length - 1] = button;
                        }
                    }
                }
            }
        }

        //何かキーが押されたら
        if (KX_netUtil.ISAnyKey())
        {
            //どのキーが押されたのか調べる
            foreach (Key code in Enum.GetValues(typeof(Key)))
            {
                if (KX_netUtil.GetISKey(code))
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

        if (!KX_netUtil.ISAnyKey()
            && (Gamepad.all.ToArray().Length > 0
            && !KX_netUtil.ISAnyPadButton(0))
            && done)
        {
            //キーバインドを適用し、設定画面を閉じる
            manager.ApplyKeyBind();
            active = false;
        }

        //入力したキーを表示
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