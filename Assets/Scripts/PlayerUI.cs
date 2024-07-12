using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    const float iconSlideIntensity = 0.3f;

    [SerializeField]
    Vector2 currentCharaIconPos;
    [SerializeField]
    float currentCharaIconSize;
    [SerializeField]
    float subCharaIconSize;
    [SerializeField]
    float tabSize;
    [SerializeField]
    float imgEXRate;

    [SerializeField]
    Canvas playPartCanvas;
    [SerializeField]
    Image[] charaImages;
    [SerializeField]
    Image[] charaImageTrays;
    [SerializeField]
    Image reviveImage;
    [SerializeField]
    TMP_Text reviveCount;
    [SerializeField]
    Canvas gameOverCanvas;
    [SerializeField]
    Canvas goalCanvas;
    [SerializeField]
    TMP_Text resultKillCount;
    [SerializeField]
    TMP_Text resultReviveCount;
    [SerializeField]
    Player player;

    void FixedUpdate()
    {
        //�������Ԃł̂ݕ\��
        playPartCanvas.enabled = player.IsLive() && !player.GetGoaled();
        //���񂾎��̂ݕ\��
        gameOverCanvas.enabled = player.IsDestructed();
        //�S�[���������̂ݕ\��
        goalCanvas.enabled = player.GetGoaled();

        //�A�C�R���̌��ɂ��鍕���g���[�̈ʒu�Ƒ傫���𒲐�
        for (int i = 0; i < Player.maxTeamNum; i++)
        {
            Image currentTray = charaImageTrays[i];
            RectTransform currentTrayRect =
                currentTray.gameObject.GetComponent<RectTransform>();

            Vector2 currentPosition = currentCharaIconPos;
            Vector3 currentScale = new Vector3(1, 1, 1);
            if (i == 0)
            {
                currentScale *= currentCharaIconSize;
            }
            else
            {
                currentPosition -= new Vector2(0,
                    (currentCharaIconSize + (subCharaIconSize) * (i - 1))
                    * imgEXRate + (tabSize) * i);
                currentScale *= subCharaIconSize;
            }
            currentTrayRect.anchoredPosition = currentPosition;
            currentTrayRect.localScale = currentScale;
        }

        //�A�C�R���̈ʒu�Ƒ傫���𒲐�
        for (int i = 0; i < Player.maxTeamNum; i++)
        {
            int currentIndex = Mathf.RoundToInt(
                Mathf.Repeat(i - player.GetCurrentCharaIndex(),
                player.GetCharacters().Length));

            Image currentTray = charaImages[i];
            RectTransform currentTrayRect =
                currentTray.gameObject.GetComponent<RectTransform>();

            Vector2 currentPosition = currentCharaIconPos;
            Vector3 currentScale = new Vector3(1, 1, 1);
            if (currentIndex == 0)
            {
                currentScale *= currentCharaIconSize;
            }
            else
            {
                currentPosition -= new Vector2(0,
                    (currentCharaIconSize + (subCharaIconSize) * (currentIndex - 1))
                    * imgEXRate + (tabSize) * currentIndex);
                currentScale *= subCharaIconSize;
            }
            currentTrayRect.anchoredPosition =
                Vector2.Lerp(currentTrayRect.anchoredPosition, currentPosition,
                iconSlideIntensity);
            currentTrayRect.localScale =
                Vector3.Lerp(currentTrayRect.localScale, currentScale,
                iconSlideIntensity);

            if (i < player.GetCharacters().Length)
            {
                currentTray.sprite = player.GetCharacters()[i].GetIconGraph();
            }
            else
            {
                currentTray.sprite = null;
                currentTrayRect.localScale = Vector3.zero;
            }
        }

        //��x������ł��Ȃ���Ε����J�E���g���\��
        reviveImage.gameObject.SetActive(player.GetReviveCount() > 0);
        //�����J�E���g�ɕ����񐔂�\��
        reviveCount.text = player.GetReviveCount().ToString();

        //���U���g�̃L���J�E���g�Ɍ��j����\��
        resultKillCount.text = player.GetKillCount().ToString();
        //���U���g�̕����J�E���g�ɂ������񐔂�\��
        resultReviveCount.text = player.GetReviveCount().ToString();
    }
}
