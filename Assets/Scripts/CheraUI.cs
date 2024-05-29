using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheraUI : MonoBehaviour
{
    //子のキャラ画像のゲームオブジェクト
    private GameObject[] charaIcon = new GameObject[6];
    //子の表示させる画像番号
    private int[] charaNumber = new int[7];
    //子の最大数
    private const int charaMaxNum = 5;

    //子のImage
    private Image[] charaImages = new Image[6];
    //更新用スプライト
    private Sprite sprite;

    //プレイヤー
    public GameObject playerObject;
    private Player player;
    //一度だけ更新するため攻撃中かどうかのフラグ
    bool isAttackNow = false;

    void Start()
    {
        player = playerObject.GetComponent<Player>();

        //子のオブジェクトデータの取得
        for(int i = 5; i < 10; i++)
        {
            charaIcon[i - 5] = transform.GetChild(i).gameObject;
            //番号初期化
            charaNumber[i - 5] = 0;
            charaImages[i - 5] = charaIcon[i - 5].GetComponent<Image>();
        }
        //6番目の画像番号だけ
        charaNumber[6] = 0;
        //デバッグ用に仮に番号入れる
        charaNumber[0] = 1;
        charaNumber[1] = 2;
        charaNumber[2] = 3;
        charaNumber[3] = 3;
        charaNumber[4] = 3;

        //画像表示
        UpdateSprite();
    }
    void Update()
    {
        CheckIsAttack();
    }
    //スプライト画像の更新
    private void UpdateSprite()
    {
        //0番なら非表示、それ以外なら番号に対応したスプライトに差し替え
        for (int i = 0; i < charaMaxNum; i++)
        {
            if (charaNumber[i] > 0)
            {
                charaIcon[i].SetActive(true);
                sprite = Resources.Load<Sprite>("CharaUI/sc" + charaNumber[i]);
                charaImages[i].sprite = sprite;
            }
            else
            {
                charaIcon[i].SetActive(false);
            }
        }
    }
    //スキル使用時のUI側での処理
    public void ChangeIcon()
    {
        for(int i = 0;i < charaMaxNum; i++)
        {
            charaNumber[i] = charaNumber[i + 1];
        }
        //全ての更新が終わった後に表示切替
        UpdateSprite();
    }
    //プレイヤーが攻撃したか監視
    private void CheckIsAttack()
    {
        if (player.GetAttackTrigger() && !isAttackNow)
        {
            ChangeIcon();
            isAttackNow = true;
        }

        if(!player.GetAttackTrigger() && isAttackNow)
        {
            isAttackNow=false;
        }
    }
}
