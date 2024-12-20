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

    KeyMap.AxisBindData axisBindData;
    public KeyMap.AxisBindData GetAxisBindData()
    {
        return axisBindData;
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
            && KX_netUtil.ISAnyPadAxis(0))
        {
            //どの軸が入力されたのか調べる
            foreach (KX_netUtil.XInputAxis axis in Enum.GetValues(typeof(KX_netUtil.XInputAxis)))
            {
                if (Mathf.Abs(KX_netUtil.GetISPadAxis(0, axis)) >= 0.5f)
                {
                    bool inverse =
                        KX_netUtil.GetISPadAxis(0, axis) < 0;

                    switch (bindPhase)
                    {
                        default:
                            axisBindData.axisX = axis;
                            axisBindData.inverseX = inverse;
                            bindPhase = 1;
                            break;
                        case 1:
                            if (axis != axisBindData.axisX)
                            {
                                axisBindData.axisY = axis;
                                axisBindData.inverseY = inverse;
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
        if (KX_netUtil.ISAnyKey()
            || (Gamepad.all.ToArray().Length > 0
            && KX_netUtil.ISAnyPadButton(0)))
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
        if (bindPhase >= 1)
        {
            bindVecName.text += "X : ";
            if (axisBindData.inverseX)
            {
                bindVecName.text += "-";
            }
            else
            {
                bindVecName.text += "+";
            }
            bindVecName.text += axisBindData.axisX.ToString() + "\n";
        }
        if (bindPhase >= 2)
        {
            bindVecName.text += "Y : ";
            if (axisBindData.inverseY)
            {
                bindVecName.text += "-";
            }
            else
            {
                bindVecName.text += "+";
            }
            bindVecName.text += axisBindData.axisY.ToString();
        }
    }
}