using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    [SerializeField]
    string quitSceneName;
    public string GetQuitSceneName()
    {
        return quitSceneName;
    }
    [SerializeField]
    bool isRushLevel;
    [SerializeField]
    Sprite backGround;
    public Sprite GetBackGround()
    {
        return backGround;
    }
    [SerializeField]
    AudioClip bgm;
    [SerializeField]
    AudioClip battleBgm;
    [SerializeField]
    AudioClip goalBgmIntro;
    [SerializeField]
    AudioClip gameOverBgmIntro;
    [SerializeField]
    AudioClip gameOverBgm;
    Player player;
    public Player GetPlayer()
    {
        return player;
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            //プレイヤーを探す
            foreach (Player obj in UnityEngine.Object.FindObjectsOfType<Player>())
            {
                player = obj;
            }
        }
        //自身のオーディオソースを探す
        AudioSource bgmSource = GetComponent<AudioSource>();

        //中ボス戦、ボス戦、雑魚ラッシュができたら下のコードを使ってバトル中であることを検出
        /*bool battling = false;
        bool annihilated = false;
        foreach (BattleField obj in UnityEngine.Object.FindObjectsOfType<BattleField>())
        {
            if (obj.gameObject.activeInHierarchy && obj.GetBattling())
            {
                battling = true;
                if (obj.GetAnnihilated())
                {
                    annihilated = true;
                }
                break;
            }
        }*/

        if (!isRushLevel)
        {
            //死んだら曲を変える
            if (player != null && !player.IsLive())
            {
                if (player.IsDestructed())
                {
                    if ((bgmSource.clip == gameOverBgmIntro
                        && !bgmSource.isPlaying)
                        || bgmSource.clip == gameOverBgm)
                    {
                        bgmSource.clip = gameOverBgm;
                    }
                    else
                    {
                        bgmSource.clip = gameOverBgmIntro;
                    }
                }
                else
                {
                    bgmSource.clip = null;
                }
            }
            //ゴールしたら曲を変える
            else if (player != null && player.GetGoaled())
            {
                bgmSource.clip = goalBgmIntro;
            }
            //中ボス戦、ボス戦、雑魚ラッシュができたら下のコードを解放
            /*else if (annihilated)
            {
                bgmSource.clip = null;
            }
            else if (battling)
            {
                bgmSource.clip = battleBgm;
            }*/
            else
            //何も起きていないときは通常のステージ曲をかける
            {
                bgmSource.clip = bgm;
            }
        }
        //フィナーレ等の特殊ステージでは通常のステージ曲をかけ続ける
        else
        {
            bgmSource.clip = bgm;
        }

        //曲を変える必要があるときに即座に曲が変わるようにする
        if (bgmSource.loop)
        {
            bgmSource.Pause();
            bgmSource.Play();
        }

        //ジングル及びイントロならループしない
        bgmSource.loop =
            bgmSource.clip != goalBgmIntro && bgmSource.clip != gameOverBgmIntro;

        //シーン切り替えを行うときに音量を小さく
        if (SceneTransition.GetSceneChangeSwitch())
        {
            bgmSource.volume =
                1 - SceneTransition.GetSceneChangeProgress();
        }
        else
        {
            bgmSource.volume = 1;
        }
    }
}
