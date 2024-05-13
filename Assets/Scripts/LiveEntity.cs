using UnityEngine;

public class LiveEntity : MonoBehaviour
{
    public struct AxisSwitch
    {
        public bool x;
        public bool y;
        public bool z;
    }

    [SerializeField]
    CharaData data;
    public CharaData GetData()
    {
        return data;
    }
    [SerializeField]
    string teamID;
    public string GetTeamID()
    {
        return teamID;
    }
    protected float drag = 0.8f;
    protected float gravityScale = 0.5f;
    protected Vector3 movement;
    protected AxisSwitch dragAxis;
    Vector3 prevPos;
    Quaternion prevRot;
    Collider currentGround;
    bool allowGroundSet;
    bool isLanding = false; //着地しているか
    public bool GetIsLanding()
    {
        return isLanding;
    }
    int maxHP;//最大体力
    int hpAmount = 1;//残り体力の割合
    bool shield;//これがtrueの間は技による無敵時間
    AttackMotionData attackMotionData;
    int attackTimeFrame;
    float attackProgress;
    public float GetAttackProgress()
    {
        return attackProgress;
    }
    float prevAttackProgress;
    AttackMotionData.Cursor[] cursors = { };


    void Awake()
    {
        prevPos = transform.position;
        prevRot = transform.rotation;
    }

    //物理演算が更新されるタイミングで毎フレーム呼ばれる
    //注意！　Update()とは呼ばれる周期が異なるため周期ズレによる不具合に気を付けて下さい
    void FixedUpdate()
    {
        if (currentGround != null)
        {
            //足を向けるべき位置を算出し、
            Vector3 localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //x軸を中心にその位置を向くように回転させる
            transform.Rotate(
                -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y) / Mathf.Deg2Rad, 0, 0, Space.Self);

            //再度足を向けるべき位置を算出し、
            localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //z軸を中心にその位置を向くように回転させる
            transform.Rotate(0, 0,
                Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y) / Mathf.Deg2Rad, Space.Self);
        }

        //バウンド防止のため変更前のmovementを保存
        Vector3 prevMovement = movement;
        //前フレームからの移動量をmovementに変換
        movement = Quaternion.Inverse(prevRot) * ((transform.position - prevPos) / Time.deltaTime);
        prevPos = transform.position;
        prevRot = transform.rotation;
        //バウンド防止処理
        if (Mathf.Sign(prevMovement.y) != Mathf.Sign(movement.y))
        {
            movement.y = 0;
        }

        //重力及び空気抵抗
        if (dragAxis.x && dragAxis.y && dragAxis.z)
        {
            movement *= drag;
        }
        else
        {
            if (dragAxis.x)
            {
                movement.x *= drag;
            }
            if (dragAxis.y)
            {
                movement.y *= drag;
            }
            if (dragAxis.z)
            {
                movement.z *= drag;
            }
        }
        movement += new Vector3(0, -gravityScale, 0);

        //allowGroundSetをリセット
        allowGroundSet = true;
        //shieldをリセット
        shield = false;

        //ここで各派生クラスの固有更新処理を呼ぶ
        LiveEntityUpdate();

        //movementをvelocityに変換
        GetComponent<Rigidbody>().velocity = transform.rotation * movement;

        //地面との接触判定を行う前に一旦着地していない状態にする
        isLanding = false;

        //prevAttackProgressを更新
        prevAttackProgress = GetAttackProgress();
        //攻撃モーションの進行度を増加
        attackProgress += 1 / Mathf.Max((float)attackTimeFrame, 1);
        attackProgress = Mathf.Clamp(attackProgress, 0, 1);
    }

    //このオブジェクトがコライダーに触れている間毎フレームこの関数が呼ばれる（触れているコライダーが自動的に引数に入る）
    //注意！　OnTriggerStay()と違って剛体同士の衝突判定専用です
    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.GetComponent<LiveEntity>() == null && allowGroundSet)
        {
            //足を向けるべき地形として登録
            currentGround = col.collider;
            // 着地判定
            isLanding = true;
        }

        //ここで各派生クラスの固有接触処理を呼ぶ
        LiveEntityCollision();
    }

    //各派生クラスの固有更新処理（派生クラス内でオーバーライドして使う）
    protected virtual void LiveEntityUpdate()
    {
    }

    //各派生クラスの固有接触処理（派生クラス内でオーバーライドして使う）
    protected virtual void LiveEntityCollision()
    {
    }

    //攻撃モーションに移行
    protected void SetAttackMotion(string name)
    {
        attackMotionData = data.SearchAttackMotion(name);
        attackTimeFrame = Mathf.Max(attackMotionData.GetData().totalFrame, 1);
        attackProgress = 0;
    }

    //攻撃モーション中か
    protected bool IsAttacking()
    {
        return attackTimeFrame < 1 || prevAttackProgress < 1;
    }
    //攻撃モーション中かつ指定の攻撃アクションを行なっているか
    protected bool IsAttacking(string name)
    {
        return IsAttacking() && attackMotionData.GetData().name == name;
    }
    //attackProgressが指定のキーポイントを通過したか
    protected bool IsHitKeyPoint(float keyPoint)
    {
        return KX_netUtil.IsIntoRange(
            keyPoint, prevAttackProgress, GetAttackProgress(),
            false, true);
    }
    //attackProgressが指定の範囲内、もしくはその範囲を1フレーム内で通過したか
    protected bool IsHitKeyPoint(Vector2 keyPoint)
    {
        return KX_netUtil.IsCrossingRange(
            prevAttackProgress, GetAttackProgress(),
            keyPoint.x, keyPoint.y,
            false, false);
    }

    //これを呼んでいる間は地形に触れてもそっちに足を向けなくなる
    protected void DisAllowGroundSet()
    {
        allowGroundSet = false;
    }

    // ダメージを受ける
    public void Damage(int damage)
    {
        if (!shield)
        {
            hpAmount -= damage / maxHP;
        }
    }

    //生きているか
    public bool IsLive()
    {
        return hpAmount > 0;
    }

    //設定されたモーションデータを読み出して実行（実行中は常に呼ぶ）
    void ExecuteAttackMotion()
    {
        if (IsAttacking())
        {
            if (attackMotionData.GetData().meleeAttackKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                        meleeAttackKeys.Length; i++)
                {
                    AttackMotionData.AttackKey current =
                        attackMotionData.GetData().meleeAttackKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        AttackMotionData.MeleeAttackData meleeAttackData =
                            attackMotionData.SearchMeleeAttackData(current.dataName);
                        MeleeAttack(meleeAttackData,
                            cursors[attackMotionData.SearchCursorIndex(current.cursorName)]);
                    }
                }
            }

            if (attackMotionData.GetData().shotKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    shotKeys.Length; i++)
                {
                    AttackMotionData.AttackKey current =
                        attackMotionData.GetData().shotKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        AttackMotionData.ShotData shotData =
                            attackMotionData.SearchShotData(current.dataName);
                        Shot(shotData,
                            cursors[attackMotionData.SearchCursorIndex(current.cursorName)]);
                    }
                }
            }

            if (attackMotionData.GetData().shieldKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                        shieldKeys.Length; i++)
                {
                    Vector2 current =
                        attackMotionData.GetData().shieldKeys[i];
                    if (IsHitKeyPoint(current))
                    {
                        shield = true;
                    }
                }
            }

            if (attackMotionData.GetData().shieldKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                        shieldKeys.Length; i++)
                {
                    Vector2 current =
                        attackMotionData.GetData().shieldKeys[i];
                    if (IsHitKeyPoint(current))
                    {
                        DisAllowGroundSet();
                    }
                }
            }

            if (attackMotionData.GetData().seKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    seKeys.Length; i++)
                {
                    AttackMotionData.SEKey current =
                        attackMotionData.GetData().seKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        GetComponent<AudioSource>().clip = current.se;
                        GetComponent<AudioSource>().Play();
                    }
                }
            }
        }
    }

    //近接および範囲攻撃
    void MeleeAttack(AttackMotionData.MeleeAttackData attackData,
        AttackMotionData.Cursor cursor)
    {

    }
    //遠距離攻撃
    void Shot(AttackMotionData.ShotData shotData,
        AttackMotionData.Cursor cursor)
    {

    }
}