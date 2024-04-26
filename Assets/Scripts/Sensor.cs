using System;
using UnityEngine;

public class Sensor : MonoBehaviour
{

    //補足　C#ではクラス型の変数は基本的にポインタ変数として扱われます　つまりnullにもなります

    LiveEntity[] targets = { };
    public LiveEntity[] GetTargets()
    {
        return targets;
    }
    LiveEntity[] tempTargets = { };

    //物理演算が更新されるタイミングで毎フレーム呼ばれる
    //注意！　Update()とは呼ばれる周期が異なるため周期ズレによる不具合に気を付けて下さい
    void FixedUpdate()
    {
        //取得用の配列に全要素代入
        targets = tempTargets;
        //再度更新する準備としてクリア
        Array.Resize(ref tempTargets, 0);
    }

    //このオブジェクトがコライダーに触れている間毎フレームこの関数が呼ばれる（触れているコライダーが自動的に引数に入る）
    //注意！　OnCollisionStay()と違ってトリガー型の接触判定専用です
    private void OnTriggerStay(Collider other)
    {
        LiveEntity tempLiveEntity =
            other.GetComponent<LiveEntity>();
        //触れたオブジェクトにLiveEntityコンポーネントがあるなら
        if (tempLiveEntity != null)
        {
            //更新用の配列に代入
            Array.Resize(ref tempTargets, tempTargets.Length + 1);
            tempTargets[tempTargets.Length - 1] = tempLiveEntity;
        }

    }
}
