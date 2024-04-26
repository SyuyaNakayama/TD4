using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//デバッグ用：選択項目が分かるようにするためだけの仮スクリプト
public class test_select_color : MonoBehaviour
{
    //色を変更するオブジェクトとそれにアタッチされてるスクリプト
    public GameObject testObj;
    private test_select tSel;
    //ステージ番号
    private int stageNum = 0;

    void Start()
    {
        tSel = testObj.GetComponent<test_select>();
    }

    void Update()
    {
        //選択項目によって色が変わるようにマテリアルの色を変更
        stageNum = tSel.GetSelectNum();
        switch (stageNum)
        {
            case 0:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 0);
                break;
            case 1:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(0, 255, 0, 0);
                break;
            case 2:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(0, 0, 255, 0);
                break;
            default:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 0);
                break;
        }
    }
}
