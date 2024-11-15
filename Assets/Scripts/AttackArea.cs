using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AttackArea : MonoBehaviour
{
    LiveEntity attacker;
    public LiveEntity GetAttacker()
    {
        return attacker;
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
    SpriteRenderer visual;
    Sprite sprite;
    bool isNewborn = true;
    bool attackAreaDataLock = false;

    void FixedUpdate()
    {
        isNewborn = false;

        //設定された画像を適用
        if (visual)
        {
            visual.sprite = sprite;
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
    public void SetSprite(Sprite setSprite)
    {
        sprite = setSprite;
    }
}