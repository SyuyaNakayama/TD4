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
        //動ける状態でのみ表示
        playPartCanvas.enabled = player.IsLive() && !player.GetGoaled();
        //死んだ時のみ表示
        gameOverCanvas.enabled = player.IsDestructed();
        //ゴールした時のみ表示
        goalCanvas.enabled = player.GetGoaled();

        //アイコンの後ろにある黒いトレーの位置と大きさを調整
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

        //アイコンの位置と大きさを調整
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

        //一度も死んでいなければ復活カウントを非表示
        reviveImage.gameObject.SetActive(player.GetReviveCount() > 0);
        //復活カウントに復活回数を表示
        reviveCount.text = player.GetReviveCount().ToString();

        //リザルトのキルカウントに撃破数を表示
        resultKillCount.text = player.GetKillCount().ToString();
        //リザルトの復活カウントにも復活回数を表示
        resultReviveCount.text = player.GetReviveCount().ToString();
    }
}
