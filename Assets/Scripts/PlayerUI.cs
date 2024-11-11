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
    Image emptyImage;
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
    LiveEntity liveEntity;

    void FixedUpdate()
    {
        //�������Ԃł̂ݕ\��
        playPartCanvas.enabled = liveEntity.IsLive() && !liveEntity.GetGoaled();
        //���񂾎��̂ݕ\��
        gameOverCanvas.enabled = liveEntity.IsDestructed();
        //�S�[���������̂ݕ\��
        goalCanvas.enabled = liveEntity.GetGoaled();

        Player player = null;

        if (liveEntity.GetCassette())
        {
            player = liveEntity.GetCassette().GetComponent<Player>();
        }
        if (player)
        {
            //EMPTY�\���̈ʒu�Ƒ傫���A�\�������^�C�~���O�𐧌�
            RectTransform emptyRect =
                    emptyImage.gameObject.GetComponent<RectTransform>();
            emptyRect.anchoredPosition = currentCharaIconPos;
            emptyRect.localScale = new Vector3(1, 1, 1) * currentCharaIconSize;

            emptyImage.enabled = player.GetAttackReactionFrame() > 0
                && player.GetWeapons().Length <= 0;

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
                    Mathf.Max(player.GetWeapons().Length, 1)));

                Image currentIcon = charaImages[i];
                RectTransform currentTrayRect =
                    currentIcon.gameObject.GetComponent<RectTransform>();

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

                if (i < player.GetWeapons().Length)
                {
                    currentIcon.sprite = player.GetWeapons()[i].GetIconGraph();
                }
                else
                {
                    currentIcon.sprite = null;
                    currentTrayRect.localScale = Vector3.zero;
                }
            }
        }

        //��x������ł��Ȃ���Ε����J�E���g���\��
        reviveImage.gameObject.SetActive(liveEntity.GetReviveCount() > 0);
        //�����J�E���g�ɕ����񐔂�\��
        reviveCount.text = liveEntity.GetReviveCount().ToString();

        //���U���g�̃L���J�E���g�Ɍ��j����\��
        resultKillCount.text = liveEntity.GetKillCount().ToString();
        //���U���g�̕����J�E���g�ɂ������񐔂�\��
        resultReviveCount.text = liveEntity.GetReviveCount().ToString();
    }
}
