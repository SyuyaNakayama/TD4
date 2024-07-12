using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackingEntity : MonoBehaviour
{
    //追尾するエンティディ
    [SerializeField]
    GameObject target;
    //ターゲットの回転用
    [SerializeField]
    GameObject rotate;
    //自身
    private GameObject me;
    //読み込ませる座標群
    private Vector3[] loadPos = new Vector3[121];
    private Quaternion[] loadRot = new Quaternion[1211];
    //データ群の最大数
    private const int dataNum = 120;
    void Start()
    {
        me = this.gameObject;
        for(int i = 0; i < dataNum; i++)
        {
            loadPos[i] = new Vector3 (0, 100, 0);
            loadRot[i] = new Quaternion(0, 0, 0, 0);
        }

    }
    void Update()
    {
        //ターゲットのデータ入力
        loadPos[0] = target.transform.position;
        loadRot[0] = rotate.transform.rotation;
        Debug.Log(rotate.transform.rotation);
        //データをずらす
        for(int i = dataNum;i > 0; i--)
        {
            loadPos[i] = loadPos[i-1];
            loadRot[i] = loadRot[i-1];
        }
        //最後尾を適用させる
        me.transform.position = new Vector3(loadPos[dataNum].x, loadPos[dataNum].y, loadPos[dataNum].z);
        me.transform.rotation = new Quaternion(loadRot[dataNum].x, loadRot[dataNum].y, loadRot[dataNum].z, loadRot[dataNum].w);
    }
}
