using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    //シーンチェンジ呼び出し元のゲームマネージャーのオブジェクトとスクリプト
    public GameObject gmObj;
    private GameManager gm;
    //ゴール
    private GameObject goal;
    //シーンチェンジまでの猶予時間
    private int goalTimer = 300;
    //取得フラグ
    private bool isGet = false;

    void Start()
    {
        gm = gmObj.GetComponent<GameManager>();
        goal = this.gameObject;
    }
    void Update()
    {
        if (isGet)
        {
            goalTimer--;
            if(goalTimer < 0)
            {
                gm.ChangeScene("stage_select");
            }
        }
    }
    //当たり判定
    private void OnTriggerEnter(Collider other)
    {
        //タグがplayerなら・一度だけ動かしたいので
        if(other.gameObject.tag == "Player" && !isGet)
        {
            //取得フラグオン
            isGet = true;
            //SetActiveで非表示にすると動かなくなるので仮で、いい方法あったら教えてくれ
            goal.transform.position = new Vector3(10000, 10000, 10000);
        }
    }
}
