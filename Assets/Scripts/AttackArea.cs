using UnityEngine;

[DisallowMultipleComponent]
public class AttackArea : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer visual;
    [SerializeField]
    BillBoard visualBillboard;
    [SerializeField]
    Transform visualRig;
    protected Transform GetVisurlRig()
    {
        return visualRig;
    }
    [SerializeField]
    AttackMotionData.AttackData data;
    public AttackMotionData.AttackData GetData()
    {
        return data;
    }
    [SerializeField]
    Vector3 blowVec;
    public Vector3 GetBlowVec()
    {
        return blowVec;
    }
    [SerializeField]
    AttackMotionData.BillboardData billboardData;
    public AttackMotionData.BillboardData GetBillboardData()
    {
        return billboardData;
    }

    LiveEntity attacker;
    public LiveEntity GetAttacker()
    {
        return attacker;
    }
    bool isNewborn = true;
    bool attackAreaDataLock = false;

    void FixedUpdate()
    {
        isNewborn = false;

        //設定された画像を適用
        if (visual)
        {
            visual.sprite = billboardData.sprite;
        }
        if (visualBillboard)
        {
            visualBillboard.yBill =
                visualBillboard.xBill = billboardData.yBillboard;
        }

        AttackAreaUpdate();
    }

    //各派生クラスの固有更新処理（派生クラス内でオーバーライドして使う）
    protected virtual void AttackAreaUpdate()
    {
    }

    public void Lock()
    {
        //生成された直後のみ実行
        if (isNewborn)
        {
            attackAreaDataLock = true;
        }
    }

    public void SetAttacker(LiveEntity setAttacker)
    {
        //ギミックの攻撃判定など、ロックされていない場合のみ実行
        if (!attackAreaDataLock)
        {
            attacker = setAttacker;
        }
    }
    public void SetData(AttackMotionData.AttackData setData, Vector3 setBlowVec)
    {
        //ロックされていない又はAttackerのFixedUpdate()内で呼ばれた場合のみ実行
        if (!attackAreaDataLock
            || attacker != null && attacker.GetCassette().GetAllowEditAttackData())
        {
            data = setData;
            blowVec = setBlowVec;
        }
    }
    public void SetBillboardData(AttackMotionData.BillboardData setBillboardData)
    {
        billboardData = setBillboardData;
    }
}