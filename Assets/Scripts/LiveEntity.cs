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
    string attackMotionID = "";
    int attackMotionFrame;
    float attackProgress;
    public float GetAttackProgress()
    {
        return attackProgress;
    }

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

        //ここで各派生クラスの固有更新処理を呼ぶ
        LiveEntityUpdate();

        //movementをvelocityに変換
        GetComponent<Rigidbody>().velocity = transform.rotation * movement;

        //地面との接触判定を行う前に一旦着地していない状態にする
        isLanding = false;

        //攻撃モーションの進行度を増加
        attackProgress += 1 / Mathf.Max((float)attackMotionFrame, 1);
        //進行度が１を超えたら攻撃モーションを止める
        if (attackProgress > 1)
        {
            attackMotionID = "";
        }
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
    protected void SetAttackMotion(string setAttackMotionID, int setAttackMotionFrame)
    {
        attackMotionID = setAttackMotionID;
        attackMotionFrame = Mathf.Max(setAttackMotionFrame, 1);
        attackProgress = 0;
    }

    //攻撃モーション中か
    public bool IsAttacking()
    {
        return attackProgress < 1 && attackMotionID != "";
    }

    //これを呼んでいる間は地形に触れてもそっちに足を向けなくなる
    protected void DisAllowGroundSet()
    {
        allowGroundSet = false;
    }
}