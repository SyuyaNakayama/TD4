using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonGuide : ControlGuide
{
    [SerializeField]
    SpriteRenderer button;
    [SerializeField]
    Image buttonImage;
    [SerializeField]
    TMP_Text label;

    protected override void ControlGuideUpdate()
    {
        Sprite buttonSprite = null;
        string labelText = "";

        int guideIndex =
            Array.IndexOf(GetTutorialPanel().GetButtonGuides(), this);
        if (guideIndex < 0)
        {
            //親の配列内に無ければ消える
            Destroy(gameObject);
        }
        else
        {
            //アイコンセットとアトラスを読み込む
            ControlIcons controlIcons =
                GetControlMapManager().SearchControlIcons(
                ControlMapManager.InputDevice.gamepad).controlIcons;
            ControlIconAtlas controlIconAtlas =
                GetControlMapManager().SearchControlIconAtlas(
                ControlMapManager.InputDevice.gamepad).controlIconAtlas;

            KX_netUtil.XInputButton[] buttons =
                GetKeyMap().GetButtons(
                GetData().useButtonInputNames[guideIndex]);
            if (buttons.Length > 0)
            {
                string iconName = controlIconAtlas.SearchButtonBindDataCellCode(
                    buttons[0].ToString()).iconName;
                //アイコンを検索
                ControlIcons.ButtonIcon buttonIcon =
                    controlIcons.SearchButtonIcon(iconName);

                if (GetCurrentFrame().pressedButtonInputNames != null
                    && Array.IndexOf(GetCurrentFrame().pressedButtonInputNames,
                    GetData().useButtonInputNames[guideIndex]) >= 0)
                {
                    buttonSprite = buttonIcon.pressed;
                }
                else
                {
                    buttonSprite = buttonIcon.nutral;
                }
                labelText = buttonIcon.label;
            }
        }

        if (button)
        {
            button.sprite = buttonSprite;
        }
        else if (buttonImage)
        {
            buttonImage.sprite = buttonSprite;
        }
        label.text = labelText;
    }
}