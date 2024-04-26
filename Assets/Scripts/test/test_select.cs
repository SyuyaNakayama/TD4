using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_select : MonoBehaviour
{
    //ゲームマネージャーのオブジェクトとスクリプト
    public GameObject gmObj;
    private GameManager gm;
    //デフォルトシーン名
    private string sceneName = "test_stage0";
    //ステージ番号、最少数と最大数
    private int stageNum = 0;
    private const int minStageNum = 0;
    private const int maxStageNum = 2;

    private void Start()
    {
        gm = gmObj.GetComponent<GameManager>();
    }
    void Update()
    {
        //左右矢印キーで選択
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            stageNum--;
            if(stageNum < minStageNum)
            {
                stageNum = maxStageNum;
            }
            ChangeTargetStage();
            Debug.Log("test_stage" +  stageNum);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            stageNum++;
            if (stageNum > maxStageNum)
            {
                stageNum = minStageNum;
            }
            ChangeTargetStage();
            Debug.Log("test_stage" + stageNum);
        }
        //スペースキーで決定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gm.ChangeScene(sceneName);
        }
    }
    //移動先のシーン名の変更
    private void ChangeTargetStage()
    {
        sceneName = "test_stage" + stageNum;
    }
    //選択中のステージ番号の取得
    public int GetSelectNum()
    {
        Debug.Log(stageNum);
        return stageNum;
    }
}
