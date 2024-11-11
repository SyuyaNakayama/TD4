using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[DisallowMultipleComponent]
public class CharacterCassette : MonoBehaviour
{
    public const float defaultMoveSpeed = 0.2f;
    const float prepareReviveLife = -0.05f;

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
    protected string facialExpressionName;
    AttackArea attackArea;
    Projectile projectile;
    LiveEntity liveEntity;
    public LiveEntity GetLiveEntity()
    {
        return liveEntity;
    }
    float moveSpeed;
    KX_netUtil.AxisSwitch moveLock;
    protected Quaternion direction;
    protected float directionAngle;
    bool directionSwitchX = true;
    bool directionSwitchY = true;
    bool directionLock;
    Quaternion visualRotation;
    protected float flipMotionAmount;
    Vector3 damageShakePos;
    int chatPos;

    public void CassetteUpdate()
    {
        liveEntity = transform.parent.GetComponent<LiveEntity>();

        if (liveEntity && liveEntity.GetIsAllowCassetteUpdate())
        {
            ResourcePalette resourcePalette = liveEntity.GetResourcePalette();
            attackArea = resourcePalette.GetAttackArea();
            projectile = resourcePalette.GetProjectile();

            transform.localScale = new Vector3(1, 1, 1);

            //prevAttackProgress���X�V
            prevAttackProgress = GetAttackProgress();
            //�U�����[�V�����̐i�s�x�𑝉�
            attackProgress += 1 / Mathf.Max((float)attackTimeFrame, 1);
            attackProgress = Mathf.Clamp(attackProgress, 0, 1);

            if (attackProgress >= 1 && prevAttackProgress >= 1)
            {
                attackMotionData = null;
            }

            liveEntity.gameObject.transform.localScale = new Vector3(data.GetScale(), data.GetScale(), data.GetScale());

            moveLock.x = false;
            moveLock.y = false;
            moveLock.z = false;

            //�v���C���Ȃ�
            if (!liveEntity.GetGoaled())
            {
                Vector2 moveInputVec = GetLiveEntity().GetControlMap().GetMoveInputVec();

                if (visual)
                {
                    //��Ԃɉ��������ʃA�j���[�V����
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
                    else if (!liveEntity.IsLanding())
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
                }

                if (!liveEntity.IsHitBacking())
                {
                    if (liveEntity.IsLive())
                    {
                        SetAttackMotion(data.SearchAttackMotion(
                            AttackMotionData.TriggerInputType.nutral));

                        if (moveInputVec.x != 0 || moveInputVec.y != 0)
                        {
                            SetAttackMotion(data.SearchAttackMotion(
                                AttackMotionData.TriggerInputType.move));
                        }

                        if (IsAttackInput())
                        {
                            SetAttackMotion(data.SearchAttackMotion(
                                AttackMotionData.TriggerInputType.tap));
                        }
                        CharaUpdate();
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
                    attackMotionData = null;
                    attackTimeFrame = 0;
                }
            }

            UpdateAttackDatas();

            //�g�����X�t�H�[�����f�t�H���g��Ԃ�
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;

            visualRotation = Quaternion.Slerp(
                visualRotation,
                Quaternion.Euler(new Vector3(0, liveEntity.GetDirection(), 0)),
                0.5f);
            transform.localRotation *= visualRotation;

            bool needDisplay = !liveEntity.IsLive()
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

    //�U�����[�V�����Ɉڍs
    protected void SetAttackMotion(string name)
    {
        SetAttackMotion(data.SearchAttackMotion(name));
    }

    //�U�����[�V�����Ɉڍs
    protected void SetAttackMotion(AttackMotionData attackMotion)
    {
        if (!attackMotionLock)
        {
            attackMotionData = attackMotion;
            attackTimeFrame = GetMaxAttackTimeFrame();
            attackProgress = 0;
        }
    }

    //�U�����[�V�����𒆒f
    protected void StopAttackMotion()
    {
        attackMotionData = null;
        attackProgress = 1;
    }

    //�U�����[�V��������
    protected bool IsAttacking()
    {
        return attackMotionData != null
            && (attackTimeFrame < 1 || prevAttackProgress < 1);
    }
    //�U�����[�V���������w��̍U���A�N�V�������s���Ă��邩
    protected bool IsAttacking(string name)
    {
        return IsAttacking() && attackMotionData.name == name;
    }
    //attackProgress���w��̃L�[�|�C���g��ʉ߂�����
    protected bool IsHitKeyPoint(float keyPoint)
    {
        return KX_netUtil.IsIntoRange(
            keyPoint, prevAttackProgress, GetAttackProgress(),
            false, true);
    }
    //attackProgress���w��͈͓̔��A�������͂��͈̔͂�1�t���[�����Œʉ߂�����
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
    //�ݒ肳�ꂽ���[�V�����f�[�^��ǂݏo���Ď��s�i���s���͏�ɌĂԁj
    void UpdateAttackMotion()
    {
        //3D�J�[�\�������Z�b�g
        Array.Resize(ref cursors, 0);

        //�ŗL����|�C���g�����Z�b�g
        Array.Resize(ref uniqueActDatas, 0);

        if (IsAttacking())
        {
            //3D�J�[�\��
            if (attackMotionData.GetCursors() != null)
            {
                cursors = attackMotionData.GetCursors();
            }

            //���ԂƋ��ɃJ�[�\�����ړ�������f�[�^��K�p
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

            //�U�����샍�b�N����
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

            //�ߐڍU��
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

            //�������U��
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

            //�ړ�
            if (attackMotionData.GetData().moveKeys != null)
            {
                Vector3 savedMovement = liveEntity.GetMovement();
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

                        replaceVector += direction * current.moveVec
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

            //�������c�铙���ړ�
            if (attackMotionData.GetData().impulseMoveKeys != null)
            {
                Vector3 savedMovement = liveEntity.GetMovement();
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

                        replaceVector += direction * current.moveVec
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

            //�������b�N����
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

            //�Z�g�p���̈ړ����x
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

            //�Z�g�p���̋�C��R
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

            //�ŗL����|�C���g
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

            //���G����
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

            //���ʉ�
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

            //�A�j���[�V����
            if (attackMotionData.GetData().animationKeys != null && visual)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    animationKeys.Length; i++)
                {
                    AttackMotionData.AnimationKey current =
                        attackMotionData.GetData().animationKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        visual.animationName = current.animationName;
                        if (!current.useOriginalAnimTime)
                        {
                            visual.animationProgress =
                                KX_netUtil.RangeMap(GetAttackProgress(),
                                current.keyFrame.x, current.keyFrame.y,
                                0, 1);
                        }
                    }
                }
            }
        }
    }
    void UpdateAttackDatas()
    {
        //�U��������o��
        //�܂��͋ߐڍU���̃f�[�^�Ɠ����������̈��p��
        Array.Resize(ref attackAreas, meleeAttackDatas.Length);
        //�̈���̍U������ɋߐڍU���̃f�[�^����
        for (int i = 0; i < attackAreas.Length; i++)
        {
            MeleeAttackAndCursorName currentData = meleeAttackDatas[i];

            //������ΐ���
            if (attackAreas[i] == null)
            {
                attackAreas[i] =
                    Instantiate(
                    liveEntity.GetResourcePalette().GetAttackArea().gameObject,
                    transform.position,
                    transform.rotation * direction,
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
                direction
                * currentData.cursor.pos;
            current.SetAttacker(liveEntity);
            current.SetData(currentData.data.attackData,
                currentData.cursor.direction);
            current.SetSprite(currentData.data.billboardData.sprite);
            current.Lock();

            allowEditAttackData = false;
        }
        //�s�v�ȍU�����������
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

        //�e���o��
        for (int i = 0; i < shotDatas.Length; i++)
        {
            ShotAndCursorName currentData = shotDatas[i];

            if (currentData.postMove)
            {
                //postMove��true�Ȃ�1�t���[���x�点��
                shotDatas[i].postMove = false;
            }
            else
            {
                if (attackMotionData)
                {
                    //����
                    Projectile current =
                        Instantiate(
                        liveEntity.GetResourcePalette().GetProjectile().gameObject,
                        transform.position,
                        transform.rotation * direction,
                        liveEntity.transform)
                        .GetComponent<Projectile>();

                    current.transform.parent = liveEntity.gameObject.transform;

                    allowEditAttackData = true;

                    float projectileScale = currentData.data.scale;
                    current.transform.localScale =
                        new Vector3(projectileScale, projectileScale, projectileScale);
                    current.transform.localPosition =
                        direction
                        * currentData.cursor.pos;
                    current.transform.localRotation =
                        direction;
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

        //�ߐځA�������U���̃f�[�^����g�p�ς݂̗v�f������
        Array.Resize(ref meleeAttackDatas, 0);

        List<ShotAndCursorName> shotDataList =
            new List<ShotAndCursorName>(shotDatas);
        shotDataList.RemoveAll(where => where.used);
        shotDatas = shotDataList.ToArray();
    }

    void HorizonMove(int direc)
    {
        if (direc != 0)
        {
            direc = direc / Mathf.Abs(direc);
        }
        liveEntity.SetMovement(liveEntity.GetMovement() + new Vector3(moveSpeed * direc, 0, 0));
    }
    void AngleMoveXY(float angle, int force = 1)
    {
        liveEntity.SetMovement(liveEntity.GetMovement() + new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * moveSpeed * force);
    }
    void AngleMoveXZ(float angle, int force = 1)
    {
        liveEntity.SetMovement(liveEntity.GetMovement() + new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * moveSpeed * force);
    }

    //�ߐڂ���є͈͍U��
    void MeleeAttack(AttackMotionData.MeleeAttackData attackData,
        AttackMotionData.Cursor cursor)
    {
        Array.Resize(ref meleeAttackDatas, meleeAttackDatas.Length + 1);
        meleeAttackDatas[meleeAttackDatas.Length - 1].data = attackData;
        meleeAttackDatas[meleeAttackDatas.Length - 1].cursor = cursor;
    }
    //�������U��
    void Shot(AttackMotionData.ShotData shotData,
        AttackMotionData.Cursor cursor, bool postMove)
    {
        Array.Resize(ref shotDatas, shotDatas.Length + 1);
        shotDatas[shotDatas.Length - 1].data = shotData;
        shotDatas[shotDatas.Length - 1].cursor = cursor;
        shotDatas[shotDatas.Length - 1].postMove = postMove;
        shotDatas[shotDatas.Length - 1].used = false;
    }

    //�ʏ�U���̓��͂��s���Ă��邩
    bool IsAttackInput()
    {
        return liveEntity.GetControlMap().GetWeaponInput();
    }
    //�w�肳�ꂽ�ŗL����|�C���g�̎��ԓ���
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