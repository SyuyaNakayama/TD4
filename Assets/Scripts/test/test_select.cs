using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class test_select : MonoBehaviour
{
    //ゲームマネージャーのオブジェクトとスクリプト
    public GameObject gmObj;
    private GameManager gm;
    //デフォルトシーン名
    private string sceneName = "stage_0";
    //ステージ番号と最大数
    public int stageNum = 0;
    private const int maxStageNum = 3;

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
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            stageNum++;
        }
        // 0~2の範囲でループさせる
        stageNum = (int)Mathf.Repeat(stageNum, maxStageNum);
        ChangeTargetStage();
        Debug.Log("stage_" + stageNum);
        //スペースキーで決定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gm.ChangeScene(sceneName);
        }
    }
    //移動先のシーン名の変更
    private void ChangeTargetStage()
    {
        sceneName = "stage_" + stageNum;
    }
    //選択中のステージ番号の取得
    public int GetSelectNum()
    {
        Debug.Log(stageNum);
        return stageNum;
    }
}
