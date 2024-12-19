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
        //�J�����u�ԃ��Z�b�g
        if (!GetPrevIsCurrentMenu())
        {
            bindPhase = 0;
            quit = false;
        }

        //�����͂��s��ꂽ��
        if (Gamepad.all.ToArray().Length > 0
            && KX_netUtil.IMAnyPadAxis(0))
        {
            //�ǂ̎������͂��ꂽ�̂����ׂ�
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

        //�L�[���{�^���������ꂽ��
        if (KX_netUtil.IMAnyKey()
            || (Gamepad.all.ToArray().Length > 0
            && KX_netUtil.IMAnyPadButton(0)))
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