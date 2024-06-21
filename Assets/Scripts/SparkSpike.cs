using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkSpike : MonoBehaviour
{
    const int flashAnimationFrameNum = 24;
    static int[] flashFrames = {0,11,15,19,21,23};

    [SerializeField]
    Texture disabledTex;
    [SerializeField]
    Texture disabledTex_flash;
    [SerializeField]
    Texture abledTex;
    [SerializeField]
    Texture abledTex_flash;
    [SerializeField]
    AttackArea attackArea;

    [SerializeField]
    int oneLoopTimeFrame = 240;
    [SerializeField]
    bool inverse;

    float progress;

    void FixedUpdate()
    {
        //oneLoopTimeFrameのフレーム数でちょうど一周するように加算
        progress = Mathf.Repeat(progress + (1f / oneLoopTimeFrame), 1);

        attackArea.gameObject.SetActive(IsActive());

        int flashProgress = Mathf.RoundToInt(Mathf.Repeat(progress * 2, 1)
            * flashAnimationFrameNum - 0.5f);
        bool flash = false;
        for (int i = 0;i < flashFrames.Length;i++)
        {
            if(flashProgress == flashFrames[i])
            {
                flash = true;
                break;
            }
        }

        Renderer renderer = GetComponent<Renderer>();
        //状態に応じて見た目を変える
        if(IsActive())
        {
            if(flash)
            {
                renderer.materials[0].SetTexture("_MainTex", abledTex_flash);
            }
            else
            {
                renderer.materials[0].SetTexture("_MainTex", abledTex);
            }
        }
        else
        {
            if(flash)
            {
                renderer.materials[0].SetTexture("_MainTex", disabledTex_flash);
            }
            else
            {
                renderer.materials[0].SetTexture("_MainTex", disabledTex);
            }
        }
    }

    bool IsActive()
    {
        return progress > 0.5f == inverse;
    }
}
