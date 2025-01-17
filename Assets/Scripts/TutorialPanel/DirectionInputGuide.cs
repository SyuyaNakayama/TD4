using System;
using UnityEngine;
using UnityEngine.UI;

public class DirectionInputGuide : ControlGuide
{
    [SerializeField]
    SpriteRenderer stick;
    [SerializeField]
    Image stickImage;

    protected override void ControlGuideUpdate()
    {
        Sprite stickSprite = null;

        int guideIndex =
            Array.IndexOf(GetTutorialPanel().GetDirectionInputGuides(), this);
        if (guideIndex < 0)
        {
            //親の配列内に無ければ消える
            Destroy(gameObject);
        }
        else
        {
            ControlIcons controlIcons =
                GetControlMapManager().SearchControlIcons(
                ControlMapManager.InputDevice.gamepad).controlIcons;

            //アイコンを検索
            ControlIcons.DirectionIcon directionIcon =
            controlIcons.SearchDirectionIcon("vecarrow");

            if (GetCurrentFrame().directionInput != Vector2Int.zero)
            {
                switch (KX_netUtil.RadToSegmentIndex(
                KX_netUtil.Atan2(GetCurrentFrame().directionInput), 8))
                {
                    default:
                        stickSprite = directionIcon.up;
                        break;
                    case 1:
                        stickSprite = directionIcon.upRight;
                        break;
                    case 2:
                        stickSprite = directionIcon.right;
                        break;
                    case 3:
                        stickSprite = directionIcon.downRight;
                        break;
                    case 4:
                        stickSprite = directionIcon.down;
                        break;
                    case 5:
                        stickSprite = directionIcon.downLeft;
                        break;
                    case 6:
                        stickSprite = directionIcon.left;
                        break;
                    case 7:
                        stickSprite = directionIcon.upLeft;
                        break;
                }
            }
            else
            {
                stickSprite = directionIcon.nutral;
            }
        }

        if (stick)
        {
            stick.sprite = stickSprite;
        }
        else if (stickImage)
        {
            stickImage.sprite = stickSprite;
        }
    }
}