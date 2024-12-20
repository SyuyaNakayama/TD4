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
        //�J�����u�ԃ��Z�b�g
        if (!GetPrevIsCurrentMenu())
        {
            bindPhase = 0;
            quit = false;
        }

        //�����͂��s��ꂽ��
        if (Gamepad.all.ToArray().Length > 0
            && KX_netUtil.ISAnyPadAxis(0))
        {
            //�ǂ̎������͂��ꂽ�̂����ׂ�
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

        //�L�[���{�^���������ꂽ��
        if (KX_netUtil.ISAnyKey()
            || (Gamepad.all.ToArray().Length > 0
            && KX_netUtil.ISAnyPadButton(0)))
        {
            quit = true;
        }
        else if (quit)
        {
            //2�̎��̃o�C���h���I����Ă����
            if (bindPhase == 2)
            {
                //�o�C���h��K�p���A�ݒ��ʂ����
                manager.ApplyVecBind();
            }
            active = false;
        }

        //���͂�������\��
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