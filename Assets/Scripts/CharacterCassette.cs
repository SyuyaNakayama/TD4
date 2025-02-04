using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class CharacterCassette : MonoBehaviour
{
    public const float defaultMoveSpeed = 0.2f;

    struct MeleeAttackAndCursorName
    {
        public AttackMotionData.Cursor cursor;
        public AttackMotionData.MeleeAttackData data;
    }
    struct ShotAndCursorName
    {
        public AttackMotionData.Cursor cursor;
        public AttackMotionData.ShotData data;
        public bool postMove;
        public bool used;
    }

    [SerializeField]
    CharaData data;
    public CharaData GetData()
    {
        return data;
    }
    [SerializeField]
    AnimationObject visual;
    public AnimationObject GetVisual()
    {
        return visual;
    }

    LiveEntity liveEntity;
    public LiveEntity GetLiveEntity()
    {
        return liveEntity;
    }
    AttackMotionData attackMotionData;
    int attackTimeFrame;
    float attackProgress;
    public float GetAttackProgress()
    {
        return attackProgress;
    }
    float prevAttackProgress;
    bool attackMotionLock;
    AttackMotionData.Cursor[] cursors = { };
    MeleeAttackAndCursorName[] meleeAttackDatas = { };
    ShotAndCursorName[] shotDatas = { };
    AttackArea[] attackAreas = { };
    string[] uniqueActDatas = { };
    bool allowEditAttackData;
    public bool GetAllowEditAttackData()
    {
        return allowEditAttackData;
    }
    GameObject[] units = { };
    bool needReplaceAnimation;
    string replaceAnimationName;
    float replaceAnimationProgress;
    float moveSpeed;
    KX_netUtil.AxisSwitch moveLock;
    bool directionLock;
    Quaternion visualRotation;
    Vector3 damageShakePos;
    int chatPos;

    public void CassetteUpdate()
    {
        liveEntity = transform.parent.GetComponent<LiveEntity>();

        if (liveEntity && liveEntity.GetIsAllowCassetteUpdate())
        {
            transform.localScale = new Vector3(1, 1, 1);

            //unitsから不要な要素を除去
            List<GameObject> unitsList =
                new List<GameObject>(units);
            unitsList.RemoveAll(where => !where);
            unitsList = unitsList.Distinct().ToList();
            units = unitsList.ToArray();

            //prevAttackProgressを更新
            prevAttackProgress = GetAttackProgress();
            //攻撃モーションの進行度を増加
            attackProgress += 1 / Mathf.Max((float)attackTimeFrame, 1);
            attackProgress = Mathf.Clamp(attackProgress, 0, 1);

            if (attackProgress >= 1 && prevAttackProgress >= 1)
            {
                StopAttackMotion();
            }

            liveEntity.gameObject.transform.localScale = new Vector3(data.GetScale(), data.GetScale(), data.GetScale());

            moveLock.x = false;
            moveLock.y = false;
            moveLock.z = false;

            //プレイ中なら
            if (!liveEntity.GetGoaled())
            {
                //メニューを開いている場合
                if (liveEntity.IsMenuPhase())
                {
                    liveEntity.SetMovement(Vector3.zero);
                }
                else
                {
                    Vector2 moveInputVec = GetLiveEntity().GetControlMap().GetMoveInputVec();

                    if (visual)
                    {
                        //状態に応じた共通アニメーション
                        if (!liveEntity.IsLive())
                        {
                            if (liveEntity.GetCadaverLifeTimeFrame() > 0)
                            {
                                visual.animationName = "defeat";
                                visual.animationProgress = Mathf.Clamp(
                                    KX_netUtil.RangeMap(
                                    liveEntity.GetCadaverLifeTimeFrame(),
                                    LiveEntity.DeadIndicateCadaverLifeTimeFrame, 0, 0, 1),
                                    0, 1);
                            }
                            else
                            {
                                visual.animationName = "dead";
                            }
                        }
                        else if (liveEntity.IsHitBacking())
                        {
                            visual.animationName = "damage";
                        }
                        else if (!liveEntity.IsLanding() && data.GetGravityScale() > 0)
                        {
                            visual.animationName = "midair";
                            visual.animationProgress = KX_netUtil.RangeMap(Mathf.Clamp(liveEntity.GetMovement().y, -3, 3), 3, -3, 0, 1);
                        }
                        else if (moveInputVec.x != 0 || moveInputVec.y != 0)
                        {
                            visual.animationName = "walk";
                        }
                        else if (liveEntity.IsPeril())
                        {
                            visual.animationName = "peril";
                        }
                        else
                        {
                            visual.animationName = "idol";
                        }

                        if (needReplaceAnimation)
                        {
                            visual.animationName = replaceAnimationName;
                            visual.animationProgress = replaceAnimationProgress;
                            needReplaceAnimation = false;
                        }
                    }

                    if (!liveEntity.IsHitBacking())
                    {
                        if (liveEntity.IsLive())
                        {
                            SetAttackMotion(
                                AttackMotionData.TriggerInputType.nutral);

                            if (moveInputVec.x != 0 || moveInputVec.y != 0)
                            {
                                SetAttackMotion(
                                    AttackMotionData.TriggerInputType.move);
                            }

                            if (IsAttackInput())
                            {
                                SetAttackMotion(
                                    AttackMotionData.TriggerInputType.tap);
                            }
                            CharaUpdate();
                        }
                        else
                        {
                            StopAttackMotion();
                            DestroyUnits();
                        }

                        attackMotionLock = false;
                        UpdateAttackMotion();

                        Vector3 holdMovement = liveEntity.GetMovement();

                        if (moveLock.x)
                        {
                            liveEntity.SetMovement(new Vector3(
                                holdMovement.x,
                                liveEntity.GetMovement().y,
                                liveEntity.GetMovement().z));
                        }
                        if (moveLock.y)
                        {
                            liveEntity.SetMovement(new Vector3(
                                liveEntity.GetMovement().x,
                                holdMovement.y,
                                liveEntity.GetMovement().z));
                        }
                        if (moveLock.z)
                        {
                            liveEntity.SetMovement(new Vector3(
                                liveEntity.GetMovement().x,
                                liveEntity.GetMovement().y,
                                holdMovement.z));
                        }

                        directionLock = false;
                    }
                    else
                    {
                        StopAttackMotion();
                        attackTimeFrame = 0;
                        DestroyUnits();
                    }
                }
            }
            else
            {
                DestroyUnits();

                if (visual)
                {
                    if (liveEntity.GetGoalAnimationTimeFrame() > 0)
                    {
                        visual.animationName = "goal";
                        visual.animationProgress =
                            KX_netUtil.RangeMap(
                            Mathf.Clamp(liveEntity.GetGoalAnimationTimeFrame(),
                            0, LiveEntity.maxGoalAnimationTimeFrame),
                            LiveEntity.maxGoalAnimationTimeFrame, 0,
                            0, 1);
                    }
                    else
                    {
                        visual.animationName = "result";
                    }
                }
            }

            UpdateAttackDatas();

            //トランスフォームをデフォルト状態に
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;

            visualRotation = Quaternion.Slerp(
                visualRotation,
                Quaternion.Euler(new Vector3(0, liveEntity.GetDirection(), 0)),
                0.5f);
            transform.localRotation *= visualRotation;

            bool needDisplay = !liveEntity.IsLive() || liveEntity.GetGoaled()
                || !liveEntity.IsGhostTime() || Time.time % 0.14f < 0.07f;

            Renderer mesh = GetComponent<Renderer>();
            if (mesh != null)
            {
                mesh.enabled = needDisplay;
            }

            if (!needDisplay)
            {
                transform.localScale = new Vector3(0, 0, 0);
            }

            if (liveEntity.GetDamageReactionTimeFrame() > 0)
            {
                if (liveEntity.GetDamageReactionTimeFrame() % 2 == 0)
                {
                    damageShakePos = new Vector3(
                        UnityEngine.Random.Range(1f, -1f),
                        UnityEngine.Random.Range(1f, -1f),
                        0).normalized * 0.1f;
                }
                transform.localPosition = damageShakePos;
            }
            else if (liveEntity.GetCadaverLifeTimeFrame() > LiveEntity.DeadIndicateCadaverLifeTimeFrame && !liveEntity.IsLive())
            {
                transform.localPosition = new Vector3(
                    Mathf.Sin(KX_netUtil.RangeMap(liveEntity.GetCadaverLifeTimeFrame(),
                    LiveEntity.MaxCadaverLifeTimeFrame,
                    LiveEntity.DeadIndicateCadaverLifeTimeFrame,
                    Mathf.PI * 10, 0))
                    * KX_netUtil.RangeMap(liveEntity.GetCadaverLifeTimeFrame(),
                    LiveEntity.MaxCadaverLifeTimeFrame,
                    LiveEntity.DeadIndicateCadaverLifeTimeFrame,
                    0.2f, 0)
                , 0, 0);
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }
        }
    }
    protected virtual void CharaUpdate()
    {
    }

    //該当する攻撃モーションがある場合はそれに移行
    protected void SetAttackMotion(string name)
    {
        if (data.IsHitAttackMotion(name))
        {
            SetAttackMotion(data.SearchAttackMotion(name));
        }
    }
    //該当する攻撃モーションがある場合はそれに移行
    protected void SetAttackMotion(AttackMotionData.TriggerInputType triggerInputType)
    {
        if (data.IsHitAttackMotion(triggerInputType))
        {
            SetAttackMotion(data.SearchAttackMotion(triggerInputType));
        }
    }

    //攻撃モーションに移行
    protected void SetAttackMotion(AttackMotionData attackMotion)
    {
        Debug.Log(attackMotionLock);
        if (!attackMotionLock)
        {
            attackMotionData = attackMotion;
            attackTimeFrame = GetMaxAttackTimeFrame();
            attackProgress = 0;
        }
    }

    //攻撃モーションを中断
    protected void StopAttackMotion()
    {
        attackMotionData = null;
        attackTimeFrame = 0;
        attackProgress = 1;
        attackMotionLock = false;
    }

    //攻撃モーション中か
    protected bool IsAttacking()
    {
        return attackMotionData != null;
    }
    //攻撃モーション中かつ指定の攻撃アクションを行っているか
    protected bool IsAttacking(string name)
    {
        return IsAttacking() && attackMotionData.name == name;
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
    int GetMaxAttackTimeFrame()
    {
        return Mathf.Max(attackMotionData.GetData().totalFrame, 1);
    }
    //設定されたモーションデータを読み出して実行（実行中は常に呼ぶ）
    void UpdateAttackMotion()
    {
        //3Dカーソルをリセット
        Array.Resize(ref cursors, 0);

        //固有動作ポイントをリセット
        Array.Resize(ref uniqueActDatas, 0);

        if (IsAttacking())
        {
            //3Dカーソル
            if (attackMotionData.GetCursors() != null)
            {
                cursors = attackMotionData.GetCursors();
            }

            //時間と共にカーソルを移動させるデータを適用
            if (attackMotionData.GetData().cursorSweepKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    cursorSweepKeys.Length; i++)
                {
                    AttackMotionData.CursorSweepKey current =
                        attackMotionData.GetData().cursorSweepKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        AttackMotionData.CursorSweepData cursorSweepData =
                            current.cursorSweepData;

                        float partProgress = KX_netUtil.Ease(KX_netUtil.RangeMap(
                            Mathf.Clamp(GetAttackProgress(),
                            current.keyFrame.x, current.keyFrame.y),
                            current.keyFrame.x, current.keyFrame.y, 0, 1),
                            cursorSweepData.easeType, cursorSweepData.easePow);

                        AttackMotionData.Cursor startCursor =
                            cursors[attackMotionData.SearchCursorIndex(
                            cursorSweepData.startCursorName)];

                        AttackMotionData.Cursor endCursor =
                            cursors[attackMotionData.SearchCursorIndex(
                            cursorSweepData.endCursorName)];

                        cursors[attackMotionData.SearchCursorIndex(
                            cursorSweepData.cursorName)].pos =
                            Vector3.Lerp(startCursor.pos, endCursor.pos,
                            partProgress);
                        cursors[attackMotionData.SearchCursorIndex(
                            cursorSweepData.cursorName)].direction =
                            Vector3.Lerp(startCursor.direction, endCursor.direction,
                            partProgress);
                    }
                }
            }

            //攻撃動作ロック時間
            if (attackMotionData.GetData().attackMotionLockKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    attackMotionLockKeys.Length; i++)
                {
                    Vector2 current =
                        attackMotionData.GetData().attackMotionLockKeys[i];
                    if (IsHitKeyPoint(current))
                    {
                        attackMotionLock = true;
                    }
                }
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
                        MeleeAttack(meleeAttackData,
                            cursors[attackMotionData.SearchCursorIndex(current.cursorName)]);
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
                        Shot(shotData,
                            cursors[attackMotionData.SearchCursorIndex(current.cursorName)],
                            current.postMove);
                    }
                }
            }

            //味方召喚
            if (attackMotionData.GetData().summonKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    summonKeys.Length; i++)
                {
                    AttackMotionData.SummonKey current =
                        attackMotionData.GetData().summonKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        Summon(transform.TransformPoint(current.localPosition),
                            transform.rotation * Quaternion.Euler(current.localEulerAngles),
                            current.inventoryCharaID, current.teamMember, current.cassetteIndex);
                    }
                }
            }

            //移動
            if (attackMotionData.GetData().moveKeys != null)
            {
                Vector3 replaceVector = Vector3.zero;
                KX_netUtil.AxisSwitch ignoreAxis =
                    new KX_netUtil.AxisSwitch();
                ignoreAxis.x = true;
                ignoreAxis.y = true;
                ignoreAxis.z = true;

                for (int i = 0; i < attackMotionData.GetData().
                    moveKeys.Length; i++)
                {
                    AttackMotionData.MoveKey current =
                        attackMotionData.GetData().moveKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        ignoreAxis.x =
                            current.ignoreAxis.x && ignoreAxis.x;
                        ignoreAxis.y =
                            current.ignoreAxis.y && ignoreAxis.y;
                        ignoreAxis.z =
                            current.ignoreAxis.z && ignoreAxis.z;

                        float key0 = KX_netUtil.Ease(KX_netUtil.RangeMap(
                            Mathf.Clamp(prevAttackProgress,
                            current.keyFrame.x, current.keyFrame.y),
                            current.keyFrame.x, current.keyFrame.y, 0, 1),
                            current.easeType, current.easePow);

                        float key1 = KX_netUtil.Ease(KX_netUtil.RangeMap(
                            Mathf.Clamp(GetAttackProgress(),
                            current.keyFrame.x, current.keyFrame.y),
                            current.keyFrame.x, current.keyFrame.y, 0, 1),
                            current.easeType, current.easePow);

                        replaceVector += liveEntity.GetDirectionQuat() * current.moveVec
                            * (key1 - key0) / Time.deltaTime;
                    }
                }

                Vector3 freezedMoveMent = liveEntity.GetMovement();
                if (!ignoreAxis.x)
                {
                    freezedMoveMent.x = 0;
                }
                if (!ignoreAxis.y)
                {
                    freezedMoveMent.y = 0;
                }
                if (!ignoreAxis.z)
                {
                    freezedMoveMent.z = 0;
                }
                liveEntity.SetMovement(freezedMoveMent + replaceVector);

                moveLock.x = !ignoreAxis.x;
                moveLock.y = !ignoreAxis.y;
                moveLock.z = !ignoreAxis.z;
            }

            //慣性が残る等速移動
            if (attackMotionData.GetData().impulseMoveKeys != null)
            {
                Vector3 replaceVector = Vector3.zero;
                KX_netUtil.AxisSwitch ignoreAxis =
                    new KX_netUtil.AxisSwitch();
                ignoreAxis.x = true;
                ignoreAxis.y = true;
                ignoreAxis.z = true;

                for (int i = 0; i < attackMotionData.GetData().
                    impulseMoveKeys.Length; i++)
                {
                    AttackMotionData.ImpulseMoveKey current =
                        attackMotionData.GetData().impulseMoveKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        ignoreAxis.x =
                            current.ignoreAxis.x && ignoreAxis.x;
                        ignoreAxis.y =
                            current.ignoreAxis.y && ignoreAxis.y;
                        ignoreAxis.z =
                            current.ignoreAxis.z && ignoreAxis.z;

                        replaceVector += liveEntity.GetDirectionQuat() * current.moveVec
                            / Time.deltaTime;
                    }
                }

                Vector3 freezedMoveMent = liveEntity.GetMovement();
                if (!ignoreAxis.x)
                {
                    freezedMoveMent.x = 0;
                }
                if (!ignoreAxis.y)
                {
                    freezedMoveMent.y = 0;
                }
                if (!ignoreAxis.z)
                {
                    freezedMoveMent.z = 0;
                }
                liveEntity.SetMovement(freezedMoveMent + replaceVector);

                moveLock.x = moveLock.x || !ignoreAxis.x;
                moveLock.y = moveLock.y || !ignoreAxis.y;
                moveLock.z = moveLock.z || !ignoreAxis.z;
            }

            //方向ロック時間
            if (attackMotionData.GetData().directionLockKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                        directionLockKeys.Length; i++)
                {
                    Vector2 current =
                        attackMotionData.GetData().directionLockKeys[i];
                    if (IsHitKeyPoint(current))
                    {
                        directionLock = true;
                    }
                }
            }

            //技使用中の移動速度
            if (attackMotionData.GetData().uniqueMoveSpeedKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    uniqueMoveSpeedKeys.Length; i++)
                {
                    AttackMotionData.UniqueMoveSpeedKey current =
                        attackMotionData.GetData().uniqueMoveSpeedKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        moveSpeed = current.moveSpeed;
                    }
                }
            }

            //技使用中の空気抵抗
            if (attackMotionData.GetData().uniqueDragKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    uniqueDragKeys.Length; i++)
                {
                    AttackMotionData.UniqueDragKey current =
                        attackMotionData.GetData().uniqueDragKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        //liveEntity.drag = current.drag;
                    }
                }
            }

            //固有動作ポイント
            if (attackMotionData.GetData().uniqueActionKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    uniqueActionKeys.Length; i++)
                {
                    AttackMotionData.UniqueActionKey current =
                        attackMotionData.GetData().uniqueActionKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        Array.Resize(ref uniqueActDatas, uniqueActDatas.Length + 1);
                        uniqueActDatas[uniqueActDatas.Length - 1] = current.uniqueActName;
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
                        liveEntity.Shield();
                    }
                }
            }

            //効果音
            if (attackMotionData.GetData().seKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    seKeys.Length; i++)
                {
                    CharaData.SEKey current =
                        attackMotionData.GetData().seKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        liveEntity.PlayAsSE(current.se);
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
                        replaceAnimationName = current.animationName;
                        if (!current.useOriginalAnimTime)
                        {
                            replaceAnimationProgress =
                                KX_netUtil.RangeMap(GetAttackProgress(),
                                current.keyFrame.x, current.keyFrame.y,
                                0, 1);
                        }
                        needReplaceAnimation = true;
                    }
                }
            }
        }
    }
    void UpdateAttackDatas()
    {
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
                    Instantiate(
                    liveEntity.GetResourcePalette().GetAttackArea().gameObject,
                    transform.position,
                    transform.rotation * liveEntity.GetDirectionQuat(),
                    liveEntity.transform)
                    .GetComponent<AttackArea>();
            }

            allowEditAttackData = true;

            AttackArea current = attackAreas[i];
            current.transform.parent = liveEntity.gameObject.transform;
            float areaScale = currentData.data.scale;
            current.transform.localScale =
                new Vector3(areaScale, areaScale, areaScale);
            current.transform.localPosition =
                liveEntity.GetDirectionQuat()
                * currentData.cursor.pos;
            current.SetAttacker(liveEntity);
            current.SetData(currentData.data.attackData,
                currentData.cursor.direction);
            current.SetSprite(currentData.data.billboardData.sprite);
            current.Lock();

            allowEditAttackData = false;
        }
        //不要な攻撃判定を消す
        for (int i = 0; i < liveEntity.transform.childCount; i++)
        {
            AttackArea current =
                liveEntity.transform.GetChild(i).GetComponent<AttackArea>();
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
                    Destroy(liveEntity.transform.GetChild(i).gameObject);
                }
            }
        }

        //弾を出す
        for (int i = 0; i < shotDatas.Length; i++)
        {
            ShotAndCursorName currentData = shotDatas[i];

            if (currentData.postMove)
            {
                //postMoveがtrueなら1フレーム遅らせる
                shotDatas[i].postMove = false;
            }
            else
            {
                if (attackMotionData)
                {
                    //生成
                    Projectile current =
                        Instantiate(
                        liveEntity.GetResourcePalette().GetProjectile().gameObject,
                        transform.position,
                        transform.rotation * liveEntity.GetDirectionQuat(),
                        liveEntity.transform)
                        .GetComponent<Projectile>();

                    current.transform.parent = liveEntity.gameObject.transform;

                    allowEditAttackData = true;

                    float projectileScale = currentData.data.scale;
                    current.transform.localScale =
                        new Vector3(projectileScale, projectileScale, projectileScale);
                    current.transform.localPosition =
                        liveEntity.GetDirectionQuat()
                        * currentData.cursor.pos;
                    current.transform.localRotation =
                        liveEntity.GetDirectionQuat();
                    current.SetAttacker(liveEntity);
                    current.SetData(currentData.data.attackData,
                        currentData.cursor.direction);
                    current.SetProjectileData(currentData.data.projectileData,
                        currentData.cursor.direction);
                    current.SetSprite(currentData.data.billboardData.sprite);

                    current.Lock();

                    allowEditAttackData = false;

                    current.transform.parent = null;
                }

                shotDatas[i].used = true;
            }
        }

        //近接、遠距離攻撃のデータから使用済みの要素を除去
        Array.Resize(ref meleeAttackDatas, 0);

        List<ShotAndCursorName> shotDataList =
            new List<ShotAndCursorName>(shotDatas);
        shotDataList.RemoveAll(where => where.used);
        shotDatas = shotDataList.ToArray();
    }

    //近接および範囲攻撃
    void MeleeAttack(AttackMotionData.MeleeAttackData attackData,
        AttackMotionData.Cursor cursor)
    {
        Array.Resize(ref meleeAttackDatas, meleeAttackDatas.Length + 1);
        meleeAttackDatas[meleeAttackDatas.Length - 1].data = attackData;
        meleeAttackDatas[meleeAttackDatas.Length - 1].cursor = cursor;
    }
    //遠距離攻撃
    void Shot(AttackMotionData.ShotData shotData,
        AttackMotionData.Cursor cursor, bool postMove)
    {
        Array.Resize(ref shotDatas, shotDatas.Length + 1);
        shotDatas[shotDatas.Length - 1].data = shotData;
        shotDatas[shotDatas.Length - 1].cursor = cursor;
        shotDatas[shotDatas.Length - 1].postMove = postMove;
        shotDatas[shotDatas.Length - 1].used = false;
    }
    //味方召喚
    void Summon(Vector3 setLocalPosition, Quaternion setLocalRotation,
        string[] inventoryCharaID, int[] teamMember, int cassetteIndex)
    {
        GameObject unit =
            LiveEntity.Spawn(liveEntity.GetResourcePalette(),
            setLocalPosition, setLocalRotation, false,
            liveEntity.GetTeamID(),
            inventoryCharaID, teamMember, cassetteIndex,
            GetLiveEntity().GetCurrentGround()).gameObject;
        AddUnits(unit);
    }

    //unitsに要素を追加
    protected void AddUnits(GameObject gameObject)
    {
        if (Array.IndexOf(units, gameObject) < 0)
        {
            Array.Resize(ref units, units.Length + 1);
            units[units.Length - 1] = gameObject;
        }
    }

    //unitsの中にあるオブジェクトを全て消去
    void DestroyUnits()
    {
        for (int i = 0; i < units.Length; i++)
        {
            Destroy(units[i]);
        }
    }

    //通常攻撃の入力を行っているか
    bool IsAttackInput()
    {
        return liveEntity.GetControlMap().GetWeaponInput();
    }
    //指定された固有動作ポイントの時間内か
    public bool IsUniqueActing(string uniqueActName)
    {
        for (int i = 0; i < uniqueActDatas.Length; i++)
        {
            if (uniqueActDatas[i] == uniqueActName)
            {
                return true;
            }
        }
        return false;
    }
}