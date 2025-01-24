using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class KeyGuide : ControlGuide
{
    [SerializeField]
    SpriteRenderer key;
    [SerializeField]
    Image keyImage;
    [SerializeField]
    TMP_Text label;
    [SerializeField]
    Sprite nutralKey;
    [SerializeField]
    Sprite pressedKey;

    protected override void ControlGuideUpdate()
    {
        Sprite keySprite = null;
        string labelText = "";

        int guideIndex =
            Array.IndexOf(GetTutorialPanel().GetKeyGuides(), this);
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
                ControlMapManager.InputDevice.keyboard).controlIcons;
            ControlIconAtlas controlIconAtlas =
                GetControlMapManager().SearchControlIconAtlas(
                ControlMapManager.InputDevice.keyboard).controlIconAtlas;

            Key[] keys =
                GetKeyMap().GetKeys(
                GetData().useButtonInputNames[guideIndex]);
            if (keys.Length > 0)
            {
                string iconName = controlIconAtlas.SearchButtonBindDataCellCode(
                    keys[0].ToString()).iconName;
                //アイコンを検索
                ControlIcons.ButtonIcon buttonIcon =
                    controlIcons.SearchButtonIcon(iconName);
                //見つからなければ作成
                if (!buttonIcon.nutral)
                {
                    buttonIcon.nutral = nutralKey;
                    buttonIcon.pressed = pressedKey;
                    buttonIcon.label = keys[0].ToString();
                }

                if (GetCurrentFrame().pressedButtonInputNames != null
                    && Array.IndexOf(GetCurrentFrame().pressedButtonInputNames,
                    GetData().useButtonInputNames[guideIndex]) >= 0)
                {
                    keySprite = buttonIcon.pressed;
                }
                else
                {
                    keySprite = buttonIcon.nutral;
                }
                labelText = buttonIcon.label;
            }
        }

        if (key)
        {
            key.sprite = keySprite;
        }
        else if (keyImage)
        {
            keyImage.sprite = keySprite;
        }
        label.text = labelText;
    }
}