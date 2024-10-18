using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackingEntity : MonoBehaviour
{
    //データ群の最大数
    const int dataNum = 20;

    //追尾するエンティディ
    [SerializeField]
    GameObject target;
    //予測線
    [SerializeField]
    LineRenderer lineRenderer;

    //読み込ませる座標群
    Vector3[] loadPos = new Vector3[dataNum];
    Quaternion[] loadRot = new Quaternion[dataNum];

    void Start()
    {
        for (int i = 0; i < dataNum; i++)
        {
            loadPos[i] = transform.position;
            loadRot[i] = transform.rotation;
        }
    }
    void FixedUpdate()
    {
        //ターゲットのデータ入力
        loadPos[0] = target.transform.position;
        loadRot[0] = target.transform.rotation;
        //データをずらす
        for (int i = dataNum - 1; i > 0; i--)
        {
            loadPos[i] = loadPos[i - 1];
            loadRot[i] = loadRot[i - 1];
        }
        //最後尾を適用させる
        transform.position = loadPos[dataNum - 1];
        transform.rotation = loadRot[dataNum - 1];
        //頂点データをlineRendererに渡す
        lineRenderer.positionCount = loadPos.Length;
        lineRenderer.SetPositions(loadPos);
    }
}
