using UnityEngine;
using System;

[DisallowMultipleComponent]
public class LiveEntity : UnLandableObject
{
    struct MeleeAttackAndCursorName
    {
        public string cursorName;
        public AttackMotionData.MeleeAttackData data;
    }
    struct ShotAndCursorName
    {
        public string cursorName;
        public AttackMotionData.ShotData data;
    }

    [System.Serializable]
    public struct TextureSendData
    {
        public MeshRenderer meshRenderer;
        public int index;
        public string propertyName;
    }
    [System.Serializable]
    public struct SpriteSendData
    {
        public SpriteRenderer spriteRenderer;
        public bool isMainSprite;
        public string propertyName;
    }

    const float cameraTiltDiffuse = 0.2f;
    const float defaultCameraDistance = 10;
    const float directionTiltIntensity = 0.5f;
    public const float minCameraAngle = 0;
    public const float maxCameraAngle = 90;
    const float goaledCameraAngle = 0;
    const float goaledCameraDistance = 3;
    const float goaledDirection = 0;
    const float ghostTimeMul = 30;
    const int reviveGhostTimeFrame = 90;
    const int maxRepairCoolTimeFrame = 600;
    const float autoRepairPower = 0.003f;
    const int maxCadaverLifeTimeFrame = 30;
    const int maxDamageReactionTimeFrame = 10;

    private GameObject gameManager;
    private MedalCounter saveMedals;

    private void Start()
    {
        gameManager = GameObject.Find("/GameManager");
        saveMedals = gameManager.GetComponent<MedalCounter>();
    }

    [SerializeField]
    ResourcePalette resourcePalette;
    [SerializeField]
    GameObject visual;
    [SerializeField]
    TextureSendData[] meshes = { };
    [SerializeField]
    SpriteSendData[] sprites = { };
    [SerializeField]
    Transform[] bodyParts = { };
    [SerializeField]
    Camera view;
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
    [SerializeField]
    Collider currentGround;
    protected float drag = 0.8f;
    Vector3 movement;
    public Vector3 GetMovement()
    {
        return movement;
    }
    Vector3 preMovement;
    public Vector3 localGrandMove
    {
        get;
        private set;
    }
    public Vector3 gimmickMove
    {
        get;
        private set;
    }
    public Vector3 gimmickMove2
    {
        get;
        private set;
    }
    protected float direction;
    float visualDirection;
    Vector3 prevPos;
    Quaternion prevRot;
    Quaternion cameraTiltRot;
    protected float cameraAngle = maxCameraAngle;
    float easedCameraAngle = maxCameraAngle;
    protected float cameraDistance = defaultCameraDistance;
    float easedCameraDistance = defaultCameraDistance;
    bool allowGroundSet;
    bool isLanding = false; //着地しているか
    public bool GetIsLanding()
    {
        return isLanding;
    }
    float hpAmount = 1;//残り体力の割合
    public float GetHPAmount()
    {
        return hpAmount;
    }
    bool shield;//これがtrueの間は技による無敵時間
    float shieldBattery;
    int hitBackTimeFrame;
    int ghostTimeFrame;//ヒット後無敵時間
    int repairCoolTimeFrame;
    int damageReactionTimeFrame;
    int cadaverLifeTimeFrame;
    int killCount;
    public int GetKillCount()
    {
        return killCount;
    }
    int reviveCount;
    public int GetReviveCount()
    {
        return reviveCount;
    }
    bool goaled;
    public bool GetGoaled()
    {
        return goaled;
    }
    AttackMotionData attackMotionData;
    int attackTimeFrame;
    float attackProgress;
    public float GetAttackProgress()
    {
        return attackProgress;
    }
    float prevAttackProgress;
    AttackMotionData.Cursor[] cursors = { };
    MeleeAttackAndCursorName[] meleeAttackDatas = { };
    ShotAndCursorName[] shotDatas = { };
    AttackArea[] attackAreas = { };
    protected string animationName;
    protected float animationProgress;
    protected string facialExpressionName;
    bool updating = false;
    public bool GetUpdating()
    {
        return updating;
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
        updating = true;

        //足を地面に向ける
        if (currentGround != null
            && currentGround.ClosestPoint(transform.position) != transform.position)
        {
            //足を向けるべき位置を算出する
            Vector3 localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //どちらかといえば縦向きに大きく回転する必要があるなら
            if (Mathf.Abs(localClosestPoint.z) > Mathf.Abs(localClosestPoint.x))
            {
                //x軸を中心にその位置を向くように回転させる
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);

                //再度足を向けるべき位置を算出し、
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //z軸を中心にその位置を向くように回転させる
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);
            }
            else
            {
                //z軸を中心にその位置を向くように回転させる
                transform.Rotate(0, 0,
                    Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                    / Mathf.Deg2Rad, Space.Self);

                //再度足を向けるべき位置を算出し、
                localClosestPoint = transform.InverseTransformPoint(
                    currentGround.ClosestPoint(transform.position));
                //x軸を中心にその位置を向くように回転させる
                transform.Rotate(
                    -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                    / Mathf.Deg2Rad, 0, 0, Space.Self);
            }
        }

        if (view != null)
        {
            //カメラの仰角値を規定範囲に収める
            cameraAngle = Mathf.Clamp(
                cameraAngle, minCameraAngle, maxCameraAngle);
            //カメラの仰角値をイージング
            easedCameraAngle = Mathf.Lerp(
                easedCameraAngle, cameraAngle, cameraTiltDiffuse);
            //カメラの距離をイージング
            easedCameraDistance = Mathf.Lerp(
                easedCameraDistance, cameraDistance, cameraTiltDiffuse);
            //前フレームからの回転の差に応じてカメラの傾き角を決める
            cameraTiltRot =
                cameraTiltRot * (prevRot * Quaternion.Inverse(transform.rotation));
            //カメラの傾き角を減衰させる
            cameraTiltRot = Quaternion.Slerp(
                cameraTiltRot, Quaternion.identity, cameraTiltDiffuse);
            //まずキャラを見下ろす角度にカメラを向ける
            view.transform.localEulerAngles = new Vector3(easedCameraAngle, 0, 0);
            //カメラの傾き角に応じてカメラを傾ける
            view.transform.rotation =
                cameraTiltRot * view.transform.rotation;
            //カメラの位置をカメラから見て後ろ側にする
            view.transform.localPosition =
                view.transform.localRotation * new Vector3(0, 0, -1)
                * easedCameraDistance;
            //カメラの距離をデフォルト値に
            cameraDistance = defaultCameraDistance;
        }

        prevRot = transform.rotation;

        //allowGroundSetをリセット
        allowGroundSet = true;
        //shieldをリセット
        shield = false;
        //アニメーションをリセット
        animationName = "";
        //表情をリセット
        facialExpressionName = "";

        //スクリプタブルオブジェクトから攻撃モーションの内容を読み出す
        UpdateAttackMotion();

        if (IsLive() && !GetGoaled())
        {
            cadaverLifeTimeFrame = maxCadaverLifeTimeFrame;

            if (IsActable())
            {
                //ここで各派生クラスの固有更新処理を呼ぶ
                LiveEntityUpdate();
            }
            else
            {
                facialExpressionName = "damage";
            }
        }
        else
        {
            //攻撃動作を解除する
            attackMotionData = null;

            //カメラを演出用の位置に調整
            cameraAngle = goaledCameraAngle;
            cameraDistance = goaledCameraDistance;
            //正面を向く
            direction = goaledDirection;

            if (cadaverLifeTimeFrame > 0)
            {
                facialExpressionName = "defeat";
                cadaverLifeTimeFrame--;
            }
            else
            {
                if (IsPlayer())
                {
                    //何かボタンを押したらゴール時はステージを出る、死亡時は復活
                    if (Input.GetKey(KeyCode.Space)
                        || Input.GetKey("joystick button 0")
                        || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)
                        || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.V)
                        || Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.N)
                        || Input.GetKey(KeyCode.M)
                        || Input.GetKey("joystick button 1"))
                        if (GetGoaled())
                        {
                            Quit();
                        }
                        else
                        {
                            Revive();
                        }
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        if (visual != null)
        {
            //体のパーツのトランスフォームをデフォルト状態に
            visual.transform.localScale = new Vector3(1, 1, 1);
            visual.transform.localPosition = Vector3.zero;
            //キャラの見た目を向いている方向へ向ける
            visualDirection += KX_netUtil.AngleDiff(visualDirection, direction)
                * directionTiltIntensity;
            visual.transform.localEulerAngles = new Vector3(0,
                visualDirection,
                0);
        }
        for (int i = 0; i < bodyParts.Length; i++)
        {
            Transform current = bodyParts[i];
            KX_netUtil.TransformData currentData =
                data.GetDefaultBodyPartsTransform(i);
            current.localPosition = currentData.position;
            current.localScale = currentData.scale;
            if (current != visual.transform)
            {
                current.localEulerAngles = currentData.eulerAngles;
            }
        }

        //現在の状態にあったアニメーションを取得
        CharaData.Animation animationData =
            data.SearchAnimation(animationName);
        //トランスフォームアニメーションを適用
        if (animationData.transformAnimationKeys != null)
        {
            for (int i = 0; i < animationData.transformAnimationKeys.Length; i++)
            {
                CharaData.TransformAnimationKey tAnimData =
                    animationData.transformAnimationKeys[i];
                if (KX_netUtil.IsIntoRange(
                    animationProgress,
                    tAnimData.keyFrame.x, tAnimData.keyFrame.y,
                    false, false))
                {
                    Transform current = bodyParts[tAnimData.bodyPartIndex];
                    float animationPartProgress =
                        KX_netUtil.Ease(KX_netUtil.RangeMap(animationProgress,
                        tAnimData.keyFrame.x, tAnimData.keyFrame.y,
                        0, 1),
                        tAnimData.easeType, tAnimData.easePow);

                    current.localPosition = Vector3.Lerp(
                        tAnimData.startTransform.position,
                        tAnimData.endTransform.position,
                        animationPartProgress);

                    if (current == visual.transform)
                    {
                        current.Rotate(Quaternion.Slerp(
                            Quaternion.Euler(tAnimData.startTransform.eulerAngles),
                            Quaternion.Euler(tAnimData.endTransform.eulerAngles),
                            animationPartProgress).eulerAngles,
                            Space.Self);
                    }
                    else
                    {
                        current.localRotation = Quaternion.Slerp(
                            Quaternion.Euler(tAnimData.startTransform.eulerAngles),
                            Quaternion.Euler(tAnimData.endTransform.eulerAngles),
                            animationPartProgress);
                    }

                    current.localScale = Vector3.Lerp(
                        tAnimData.startTransform.scale,
                        tAnimData.endTransform.scale,
                        animationPartProgress);
                }
            }
        }
        if (animationData.facialExpressionKeys != null)
        {
            for (int i = 0; i < animationData.facialExpressionKeys.Length; i++)
            {
                CharaData.FacialExpressionKey fKeyData =
                    animationData.facialExpressionKeys[i];
                if (KX_netUtil.IsIntoRange(
                    animationProgress,
                    fKeyData.keyFrame.x, fKeyData.keyFrame.y,
                    false, false))
                {
                    facialExpressionName = fKeyData.facialExpressionName;
                }
            }
        }

        //デフォルトのテクスチャをモデルに貼る
        for (int i = 0; i < meshes.Length; i++)
        {
            TextureSendData current = meshes[i];
            current.meshRenderer.materials[current.index].
                SetTexture(current.propertyName, data.GetDefaultTexture(i));
        }
        //デフォルトのスプライトをスプライトレンダラーに貼る
        for (int i = 0; i < sprites.Length; i++)
        {
            SpriteSendData current = sprites[i];
            if (current.isMainSprite)
            {
                current.spriteRenderer.sprite = data.GetDefaultSprite(i);
            }
            else
            {
                current.spriteRenderer.material.
                    SetTexture(current.propertyName, data.GetDefaultSprite(i).texture);
            }
        }

        //現在の状態にあった表情を取得
        CharaData.FacialExpression facialData =
            data.SearchFacialExpression(facialExpressionName);
        //表情のテクスチャをモデルに貼る
        if (facialData.indexAndTextures != null)
        {
            for (int i = 0; i < facialData.indexAndTextures.Length; i++)
            {
                CharaData.IndexAndTexture texData = facialData.indexAndTextures[i];
                TextureSendData current = meshes[texData.index];
                current.meshRenderer.materials[current.index].
                    SetTexture(current.propertyName, texData.texture);
            }
        }
        //表情のスプライトをスプライトレンダラーに貼る
        if (facialData.indexAndSprites != null)
        {
            for (int i = 0; i < facialData.indexAndSprites.Length; i++)
            {
                CharaData.IndexAndSprite spriteData = facialData.indexAndSprites[i];
                SpriteSendData current = sprites[spriteData.index];
                if (current.isMainSprite)
                {
                    current.spriteRenderer.sprite = spriteData.sprite;
                }
                else
                {
                    current.spriteRenderer.material.
                        SetTexture(current.propertyName, spriteData.sprite.texture);
                }
            }
        }

        if (visual != null)
        {
            //ヒット後無敵時間中なら点滅
            if ((ghostTimeFrame > 0 && Time.time % 0.1f < 0.05f) && IsLive()
            || IsDestructed())
            {
                visual.transform.localScale = Vector3.zero;
            }
            //攻撃を受けた直後ならシェイク
            if (damageReactionTimeFrame > 0)
            {
                visual.transform.localPosition +=
                    Vector3.Normalize(new Vector3(UnityEngine.Random.Range(1f, -1f),
                    UnityEngine.Random.Range(1f, -1f),
                    UnityEngine.Random.Range(1f, -1f))) * 0.2f; ;
            }
        }

        //prevAttackProgressを更新
        prevAttackProgress = GetAttackProgress();
        //攻撃モーションの進行度を増加
        attackProgress += 1 / Mathf.Max((float)attackTimeFrame, 1);
        attackProgress = Mathf.Clamp(attackProgress, 0, 1);

        ghostTimeFrame = Mathf.Max(0, ghostTimeFrame - 1);
        hitBackTimeFrame = Mathf.Max(0, hitBackTimeFrame - 1);
        damageReactionTimeFrame =
            Mathf.Max(0, damageReactionTimeFrame - 1);

        //【重要】ここから「preMovement = movement;」までmovementの値を書き換えないこと
        //movementをvelocityに変換
        GetComponent<Rigidbody>().velocity =
            transform.rotation * movement * transform.localScale.x;

        Vector3 playerLocalPosPin = transform.InverseTransformPoint(prevPos);
        prevPos = transform.position;

        //ギミックによる移動に関する更新処理
        GetComponent<Rigidbody>().velocity += gimmickMove;
        playerLocalPosPin += transform.InverseTransformPoint(transform.position + gimmickMove * Time.deltaTime);
        localGrandMove = -playerLocalPosPin;
        gimmickMove = Vector3.zero;

        GetComponent<Rigidbody>().velocity += gimmickMove2;
        gimmickMove2 = Vector3.zero;

        Vector3 movementDiff = movement - preMovement;
        preMovement = movement;
        //これ以降はmovementの値を書き換えて良い

        //着地判定
        Vector3 pushBackedMovement =
            localGrandMove / Time.deltaTime + movementDiff;

        if (Vector3.Magnitude(pushBackedMovement) < Vector3.Magnitude(movement))
        {
            movement = Vector3.Lerp(movement, pushBackedMovement, 0.5f);
        }

        //空気抵抗
        KX_netUtil.AxisSwitch dragAxis = data.GetDragAxis();
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
        //重力
        movement += new Vector3(0, -data.GetGravityScale(), 0);

        //地面との接触判定を行う前に一旦着地していない状態にする
        isLanding = false;

        //ゴールしたら無敵に
        if (GetGoaled())
        {
            ghostTimeFrame = reviveGhostTimeFrame;
        }

        //しばらくダメージを受けていなければ回復
        if (IsLive() && IsDamageTakeable())
        {
            repairCoolTimeFrame--;
            if (repairCoolTimeFrame <= 0)
            {
                hpAmount += autoRepairPower;
            }
        }
        repairCoolTimeFrame = Mathf.RoundToInt(
            Mathf.Clamp(repairCoolTimeFrame, 0, maxRepairCoolTimeFrame));

        hpAmount = Mathf.Clamp(hpAmount, 0, 1);

        updating = false;
    }

    //このオブジェクトがコライダーに触れている間毎フレームこの関数が呼ばれる（触れているコライダーが自動的に引数に入る）
    //注意！　OnTriggerStay()と違って剛体同士の衝突判定専用です
    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.GetComponent<AttackArea>() != null)
        {
            AttackHit(col.gameObject.GetComponent<AttackArea>());
        }
        if (col.gameObject.GetComponent<UnLandableObject>() == null && allowGroundSet)
        {
            //足を向けるべき地形として登録
            currentGround = col.collider;
            // 着地判定
            isLanding = true;
        }

        //ここで各派生クラスの固有接触処理を呼ぶ
        LiveEntityOnHit(col.collider);
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.GetComponent<AttackArea>() != null)
        {
            AttackHit(col.gameObject.GetComponent<AttackArea>());
        }
        if (col.gameObject.GetComponent<Goal>() != null && IsPlayer())
        {
            Clear();
            saveMedals.Save();
        }

        //ここで各派生クラスの固有接触処理を呼ぶ
        LiveEntityOnHit(col);
    }

    //各派生クラスの固有更新処理（派生クラス内でオーバーライドして使う）
    protected virtual void LiveEntityUpdate()
    {
    }

    //TODO:開発終盤で必要か否か判断し、不要なら消す
    //各派生クラスの固有衝突処理（派生クラス内でオーバーライドして使う）
    protected virtual void LiveEntityOnHit(Collider col)
    {

    }

    //攻撃モーションに移行
    protected void SetAttackMotion(string name)
    {
        SetAttackMotion(data.SearchAttackMotion(name));
    }

    //攻撃モーションに移行
    protected void SetAttackMotion(AttackMotionData attackMotion)
    {
        attackMotionData = attackMotion;
        attackTimeFrame = Mathf.Max(attackMotionData.GetData().totalFrame, 1);
        attackProgress = 0;
    }

    //攻撃モーション中か
    protected bool IsAttacking()
    {
        return attackMotionData != null
            && (attackTimeFrame < 1 || prevAttackProgress < 1);
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

    //攻撃を受けた際にこれを呼ぶ
    void AttackHit(AttackArea attackArea)
    {
        LiveEntity attacker = attackArea.GetAttacker();

        //攻撃を受け付ける状態、かつ味方以外からの攻撃なら
        if (IsLive() && !shield && ghostTimeFrame <= 0
            && (attacker == null
                || attacker.GetTeamID() != teamID))
        {
            //ギミックならデータ上の数値をそのまま使う
            float damageValue = attackArea.GetData().power;
            int ghostTime = attackArea.GetData().ghostTime;
            //キャラの攻撃ならダメージ値と無敵時間を算出
            if (attacker != null)
            {
                float attackerPower =
                    attacker.GetData().GetAttackPower();
                damageValue *= attackerPower;
                ghostTime = Mathf.RoundToInt(
                    damageValue / attackerPower * ghostTimeMul);
            }
            Damage(damageValue, ghostTime);

            if (!IsLive() && attacker != null)
            {
                attacker.killCount++;
            }
            HitBack(attackArea.GetBlowVec(), attackArea.GetData().hitback);
        }
    }
    //体力を減らし、無敵時間を付与
    void Damage(float damage, int setGhostTimeFrame)
    {
        hpAmount -= Mathf.Max(0, damage / data.GetLife());
        ghostTimeFrame = setGhostTimeFrame;
        repairCoolTimeFrame = maxRepairCoolTimeFrame;
        damageReactionTimeFrame = maxDamageReactionTimeFrame;
        //ダメージ音を鳴らす
        if (IsLive())
        {
            PlayAsSE(resourcePalette.GetDamageSE());
        }
        else
        {
            PlayAsSE(resourcePalette.GetDefeatSE());
        }
    }
    //吹っ飛ばされる
    void HitBack(Vector3 hitBackVec, int setHitBackTimeFrame)
    {
        movement = Quaternion.Inverse(transform.rotation)
            * hitBackVec;
        hitBackTimeFrame = setHitBackTimeFrame;
        attackMotionData = null;
    }

    //生きているか
    public bool IsLive()
    {
        return hpAmount > 0;
    }
    //技による無敵状態か
    public bool IsShield()
    {
        return /*shieldable &&*/ shield;
    }
    //ダメージを受け付ける状態か
    public bool IsDamageTakeable()
    {
        return !IsShield() && ghostTimeFrame <= 0;
    }
    //行動できる状態か
    public bool IsActable()
    {
        return hitBackTimeFrame <= 0;
    }
    //これはプレイヤーか
    public bool IsPlayer()
    {
        return GetComponent<Player>() != null;
    }
    public bool IsDestructed()
    {
        return !IsLive() && cadaverLifeTimeFrame <= 0;
    }

    //死んでいるときにこれを呼ぶと復活する
    void Revive()
    {
        if (!IsLive())
        {
            hpAmount = 1;
            hitBackTimeFrame = 0;
            ghostTimeFrame = reviveGhostTimeFrame;
            reviveCount++;
        }
    }
    //ゴールに入った時の処理
    void Clear()
    {
        goaled = true;
    }
    //今いるステージの派生元として設定されているシーンに戻る
    void Quit()
    {
        foreach (StageManager obj in UnityEngine.Object.FindObjectsOfType<StageManager>())
        {
            if (obj.gameObject.activeInHierarchy)
            {
                SceneTransition.ChangeScene(obj.GetQuitSceneName());
                return;
            }
        }
    }

    //設定されたモーションデータを読み出して実行（実行中は常に呼ぶ）
    void UpdateAttackMotion()
    {
        //近接、遠距離攻撃のデータをリセット
        Array.Resize(ref meleeAttackDatas, 0);
        Array.Resize(ref shotDatas, 0);
        //3Dカーソルをリセット
        Array.Resize(ref cursors, 0);

        if (IsAttacking())
        {
            //3Dカーソルを取得
            if (attackMotionData.GetCursors() != null)
            {
                cursors = attackMotionData.GetCursors();
            }

            //近接攻撃
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
                        MeleeAttack(meleeAttackData, current.cursorName);
                    }
                }
            }

            //遠距離攻撃
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
                        Shot(shotData, current.cursorName);
                    }
                }
            }

            //移動
            if (attackMotionData.GetData().moveKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                        moveKeys.Length; i++)
                {
                    AttackMotionData.MoveKey current =
                        attackMotionData.GetData().moveKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        float key0 = KX_netUtil.Ease(KX_netUtil.RangeMap(prevAttackProgress,
                            current.keyFrame.x, current.keyFrame.y, 0, 1),
                            current.easeType, current.easePow);

                        float key1 = KX_netUtil.Ease(KX_netUtil.RangeMap(GetAttackProgress(),
                            current.keyFrame.x, current.keyFrame.y, 0, 1),
                            current.easeType, current.easePow);

                        movement = Quaternion.Euler(new Vector3(0, direction, 0))
                            * current.moveVec * (key1 - key0) / Time.deltaTime;
                    }
                }
            }

            //無敵時間
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

            //着陸不可時間
            if (attackMotionData.GetData().disAllowGroundSetKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                        disAllowGroundSetKeys.Length; i++)
                {
                    Vector2 current =
                        attackMotionData.GetData().disAllowGroundSetKeys[i];
                    if (IsHitKeyPoint(current))
                    {
                        DisAllowGroundSet();
                    }
                }
            }

            //効果音
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

            //アニメーション
            if (attackMotionData.GetData().animationKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    animationKeys.Length; i++)
                {
                    AttackMotionData.AnimationKey current =
                        attackMotionData.GetData().animationKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        animationName = current.animationName;
                        animationProgress =
                            KX_netUtil.RangeMap(GetAttackProgress(),
                            current.keyFrame.x, current.keyFrame.y,
                            0, 1);
                    }
                }
            }
        }

        //攻撃判定を出す
        //まずは近接攻撃のデータと同じ数だけ領域を用意
        Array.Resize(ref attackAreas, meleeAttackDatas.Length);
        //領域内の攻撃判定に近接攻撃のデータを代入
        for (int i = 0; i < attackAreas.Length; i++)
        {
            MeleeAttackAndCursorName currentData = meleeAttackDatas[i];

            //無ければ生成
            if (attackAreas[i] == null)
            {
                attackAreas[i] =
                    Instantiate(resourcePalette.GetAttackArea().gameObject,
                    transform.position, transform.rotation, transform)
                    .GetComponent<AttackArea>();
            }

            AttackArea current = attackAreas[i];
            current.transform.parent = gameObject.transform;
            float areaScale = currentData.data.scale;
            current.transform.localScale =
                new Vector3(areaScale, areaScale, areaScale);
            current.transform.localPosition =
                Quaternion.Euler(new Vector3(0, direction, 0))
                * cursors[attackMotionData.SearchCursorIndex(currentData.cursorName)].pos;
            current.SetAttacker(this);
            current.SetData(currentData.data.attackData,
                cursors[attackMotionData.SearchCursorIndex(currentData.cursorName)].direction);
        }
        //不要な攻撃判定を消す
        for (int i = 0; i < transform.childCount; i++)
        {
            AttackArea current =
                transform.GetChild(i).GetComponent<AttackArea>();
            if (current != null)
            {
                bool needDestroy = true;
                for (int j = 0; j < attackAreas.Length; j++)
                {
                    if (current == attackAreas[j])
                    {
                        needDestroy = false;
                        break;
                    }
                }
                if (needDestroy)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }

        //弾を出す
        for (int i = 0; i < shotDatas.Length; i++)
        {
            ShotAndCursorName currentData = shotDatas[i];

            //生成
            Projectile current =
                    Instantiate(resourcePalette.GetProjectile().gameObject,
                    transform.position, transform.rotation, transform)
                    .GetComponent<Projectile>();

            current.transform.parent = gameObject.transform;

            float projectileScale = currentData.data.scale;
            current.transform.localScale =
                new Vector3(projectileScale, projectileScale, projectileScale);
            current.transform.localPosition =
                Quaternion.Euler(new Vector3(0, direction, 0))
                * cursors[attackMotionData.SearchCursorIndex(currentData.cursorName)].pos;
            current.transform.localRotation =
                Quaternion.Euler(new Vector3(0, direction, 0));
            current.SetAttacker(this);
            current.SetData(currentData.data.attackData,
                cursors[attackMotionData.SearchCursorIndex(currentData.cursorName)].direction);
            current.SetProjectileData(currentData.data.projectileData,
                cursors[attackMotionData.SearchCursorIndex(currentData.cursorName)].direction);

            current.transform.parent = null;
        }
    }

    //近接および範囲攻撃
    void MeleeAttack(AttackMotionData.MeleeAttackData attackData, string cursorName)
    {
        Array.Resize(ref meleeAttackDatas, meleeAttackDatas.Length + 1);
        meleeAttackDatas[meleeAttackDatas.Length - 1].data = attackData;
        meleeAttackDatas[meleeAttackDatas.Length - 1].cursorName = cursorName;
    }
    //遠距離攻撃
    void Shot(AttackMotionData.ShotData shotData, string cursorName)
    {
        Array.Resize(ref shotDatas, shotDatas.Length + 1);
        shotDatas[shotDatas.Length - 1].data = shotData;
        shotDatas[shotDatas.Length - 1].cursorName = cursorName;
    }
    //動ける状態なら移動
    public void Move(Vector3 setMovement)
    {
        if (IsActable())
        {
            movement = setMovement;
        }
    }
    //リフトに乗っている時や風に煽られているの動きを実現するための関数
    public void AddFieldMove(Vector3 force)
    {
        gimmickMove += force;
    }
    //擬似的に壁に押されたような動きを実現するための関数
    public void AddPushBackMove(Vector3 force)
    {
        gimmickMove2 += force;
    }
    //このLiveEntityから効果音を鳴らす
    public void PlayAsSE(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
    }
}