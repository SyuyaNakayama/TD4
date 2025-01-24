using System;
using UnityEngine;
using UnityEngine.UI;

public class TouchGuide : ControlGuide
{
    const float nearVecLength = 0.3f;
    const float farVecLength = 0.6f;

    [SerializeField]
    SpriteRenderer touchPanel;
    [SerializeField]
    SpriteRenderer hand;
    [SerializeField]
    Image touchPanelImage;
    [SerializeField]
    Image handImage;
    [SerializeField]
    Sprite nutralHand;
    [SerializeField]
    Sprite touchHand;
    [SerializeField]
    Sprite nutralHoloStick;
    [SerializeField]
    Sprite SecondTriggerHoloStick;
    [SerializeField]
    Sprite characterImg;

    protected override void ControlGuideUpdate()
    {
        Sprite touchPanelSprite = null;
        Sprite handSprite = null;

        int guideIndex =
            Array.IndexOf(GetTutorialPanel().GetTouchGuides(), this);
        if (guideIndex < 0)
        {
            //親の配列内に無ければ消える
            Destroy(gameObject);
        }
        else
        {
            //アニメーション全体を通してサブボタンを使うか
            bool useSubButton = false;
            for (int i = 0; i < GetData().useButtonInputNames.Length; i++)
            {
                if (GetData().useButtonInputNames[i] == "sub")
                {
                    useSubButton = true;
                    break;
                }
            }

            //アニメーション内の今のフレームでメインボタンやサブボタンを押しているか
            bool mainButtonPressed = false;
            if (GetCurrentFrame().pressedButtonInputNames != null)
            {
                for (int i = 0; i < GetCurrentFrame().pressedButtonInputNames.Length; i++)
                {
                    string latestInputName =
                        GetCurrentFrame().pressedButtonInputNames[i];
                    if (latestInputName == "main" || latestInputName == "sub")
                    {
                        mainButtonPressed = true;
                    }
                }
            }

            Vector2 inputVec = ((Vector2)GetCurrentFrame().directionInput).normalized;
            if (mainButtonPressed)
            {
                inputVec *= nearVecLength;
            }
            else
            {
                inputVec *= farVecLength;
            }

            bool inputDetected =
                mainButtonPressed || inputVec != Vector2.zero;

            Transform handTransform = null;
            if (hand)
            {
                handTransform = hand.transform;
            }
            else if (handImage)
            {
                handTransform = handImage.transform;
            }

            if (inputDetected)
            {
                handSprite = touchHand;
                handTransform.localPosition = Vector3.Lerp(
                    handTransform.localPosition, inputVec, 0.7f);
            }
            else
            {
                handSprite = nutralHand;
            }
            //アニメーションの初めに真ん中に戻す
            if (GetTutorialPanel().GetProgress() < GetTutorialPanel().GetPrevProgress())
            {
                handTransform.localPosition = Vector3.zero;
            }

            if (useSubButton)
            {
                if (inputDetected)
                {
                    touchPanelSprite = SecondTriggerHoloStick;
                }
                else
                {
                    touchPanelSprite = characterImg;
                }
            }
            else if (inputDetected)
            {
                touchPanelSprite = nutralHoloStick;
            }
        }

        if (touchPanel && hand)
        {
            touchPanel.sprite = touchPanelSprite;
            hand.sprite = handSprite;
        }
        else if (touchPanelImage && handImage)
        {
            touchPanelImage.sprite = touchPanelSprite;
            handImage.sprite = handSprite;
        }
    }
}