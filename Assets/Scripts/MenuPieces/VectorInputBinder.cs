using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class VectorInputBinder : Menu
{
    [SerializeField]
    ControlMapManager manager;
    [SerializeField]
    TMP_Text bindVecName;
    [SerializeField]
    string vecCellName;
    public string GetVecCellName()
    {
        return vecCellName;
    }

    KX_netUtil.XInputAxis bindAxisX;
    public KX_netUtil.XInputAxis GetBindAxisX()
    {
        return bindAxisX;
    }
    KX_netUtil.XInputAxis bindAxisY;
    public KX_netUtil.XInputAxis GetBindAxisY()
    {
        return bindAxisY;
    }
    int bindPhase;
    bool quit;

    protected override void MenuUpdate()
    {
        //開いた瞬間リセット
        if (!GetPrevIsCurrentMenu())
        {
            bindPhase = 0;
            quit = false;
        }

        //軸入力が行われたら
        if (Gamepad.all.ToArray().Length > 0
            && KX_netUtil.IMAnyPadAxis(0))
        {
            //どの軸が入力されたのか調べる
            foreach (KX_netUtil.XInputAxis axis in Enum.GetValues(typeof(KX_netUtil.XInputAxis)))
            {
                if (Mathf.Abs(KX_netUtil.GetIMPadAxis(0, axis)) >= 0.5f)
                {
                    switch (bindPhase)
                    {
                        default:
                            bindAxisX = axis;
                            bindPhase = 1;
                            break;
                        case 1:
                            if (axis != bindAxisX)
                            {
                                bindAxisY = axis;
                                bindPhase = 2;
                            }
                            break;
                        case 2:
                            break;
                    }
                }
            }
        }

        //キーかボタンが押されたら
        if (KX_netUtil.IMAnyKey()
            || (Gamepad.all.ToArray().Length > 0
            && KX_netUtil.IMAnyPadButton(0)))
        {
            quit = true;
        }
        else if (quit)
        {
            //2つの軸のバインドが終わっていれば
            if (bindPhase == 2)
            {
                //バインドを適用し、設定画面を閉じる
                manager.ApplyVecBind();
            }
            active = false;
        }

        //入力した軸を表示
        bindVecName.text = "";
        switch (bindPhase)
        {
            case 1:
                bindVecName.text += "X : " + bindAxisX.ToString();
                break;
            case 2:
                bindVecName.text += "X : " + bindAxisX.ToString()
                    + "\nY : " + bindAxisY.ToString();
                break;
        }
    }
}