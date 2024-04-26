using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LiveEntity
{
    [SerializeField]
    Sensor sensor;
    protected override void LiveEntityUpdate()
    {
        //ここで各派生クラスの固有更新処理を呼ぶ
        EnemyUpdate();
    }

    //各派生クラスの固有更新処理（派生クラス内でオーバーライドして使う）
    protected virtual void EnemyUpdate()
    {
    }
}