using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_scene_change : MonoBehaviour
{
    //ゲームマネージャーのオブジェクトとそのスクリプト
    public GameObject gmObj;
    private GameManager gm;
    //移動させたいシーン名
    public string sceneName;

    private void Start()
    {
        gm = gmObj.GetComponent<GameManager>();
    }


    void Update()
    {
        //スペースキーでシーン移動
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gm.ChangeScene(sceneName);
        }
        //※デバッグ用：Sキーでテスト用マップに移動
        if (Input.GetKeyDown(KeyCode.S))
        {
            gm.ChangeScene("SampleScene");
        }
    }
}
