using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaUI : MonoBehaviour
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
    Image[] charaImages;
    [SerializeField]
    Image[] charaImageTrays;
    [SerializeField]
    Player player;

    void FixedUpdate()
    {
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

            currentTray.sprite = player.GetCharacters()[i].GetIconGraph();
        }
    }
}
