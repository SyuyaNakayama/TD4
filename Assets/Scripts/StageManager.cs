using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    static StageManager current;
    public static StageManager GetCurrent()
    {
        return current;
    }

    [SerializeField]
    string quitSceneName;
    public string GetQuitSceneName()
    {
        return quitSceneName;
    }
    [SerializeField]
    string nextStageName;
    public string GetNextStageName()
    {
        return nextStageName;
    }
    [SerializeField]
    bool isRushLevel;
    [SerializeField]
    Sprite backGround;
    [SerializeField]
    Material groundMaterial;
    [SerializeField]
    Material unLandableMaterial;
    [SerializeField]
    string texPropertyName;
    [SerializeField]
    Texture groundTex;
    [SerializeField]
    Texture unLandableObjectTex;
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
    LiveEntity player;

    void FixedUpdate()
    {
        //現在のインスタンスを入れる変数を更新
        current = this;

        //床のテクスチャを貼り替える
        groundMaterial.SetTexture(texPropertyName, groundTex);
        //UnLandableObjectのテクスチャを貼り替える
        unLandableMaterial.SetTexture(texPropertyName, unLandableObjectTex);

        //プレイヤーを探す
        foreach (LiveEntity obj in LiveEntity.GetAllInstances())
        {
            if (obj && obj.GetUserControl())
            {
                player = obj;
            }
        }

        //自身のオーディオソースを探す
        AudioSource bgmSource = GetComponent<AudioSource>();

        //バトル中であることを検出
        bool battling = false;
        bool annihilated = false;
        foreach (BattleField obj in BattleField.GetAllInstances())
        {
            if (obj && obj.gameObject.activeInHierarchy && obj.GetBattling())
            {
                battling = true;
                if (obj.GetAnnihilated())
                {
                    annihilated = true;
                }
                break;
            }
        }

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
            //戦闘中は曲を変える
            else if (annihilated)
            {
                bgmSource.clip = null;
            }
            else if (battling)
            {
                bgmSource.clip = battleBgm;
            }
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
