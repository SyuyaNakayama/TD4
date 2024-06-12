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
    float progress;

    void FixedUpdate()
    {
        //oneLoopTimeFrameのフレーム数でちょうど一周するように加算
        progress = Mathf.Repeat(progress + (1f / oneLoopTimeFrame), 1);

        attackArea.gameObject.SetActive(progress > 0.5f);

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
        if(progress > 0.5f)
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

    void OnWillRenderObject()
    {
        Vector2 tiling = new Vector2(8 * Screen.width / (float)Screen.height, 8);
        GetComponent<Renderer>().materials[0].SetTextureScale("_MainTex", tiling);
    }
}
