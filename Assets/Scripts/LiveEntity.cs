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
    const int maxRepairCoolTimeFrame = 780;
    const int maxCadaverLifeTimeFrame = 30;
    const int maxDamageReactionTimeFrame = 10;

    [SerializeField]
    ResourcePalette resourcePalette;
    [SerializeField]
    GameObject visual;
    [SerializeField]
    Camera view;
    [SerializeField]
    SpriteRenderer lifeGauge;
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
    protected float direction;
    Vector3 prevPos;
    Quaternion prevRot;
    Quaternion cameraTiltRot;
    protected float cameraAngle = maxCameraAngle;
    float easedCameraAngle = maxCameraAngle;
    protected float cameraDistance = defaultCameraDistance;
    float easedCameraDistance = defaultCameraDistance;
    bool allowGroundSet;
    bool isLanding = false; //���n���Ă��邩
    public bool GetIsLanding()
    {
        return isLanding;
    }
    float maxHP;//�ő�̗�
    float hpAmount = 1;//�c��̗͂̊���
    bool shield;//���ꂪtrue�̊Ԃ͋Z�ɂ�閳�G����
    float shieldBattery;
    int hitBackTimeFrame;
    int ghostTimeFrame;//�q�b�g�㖳�G����
    int repairCoolTimeFrame;
    int damageReactionTimeFrame;
    int cadaverLifeTimeFrame;
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

    //�������Z���X�V�����^�C�~���O�Ŗ��t���[���Ă΂��
    //���ӁI�@Update()�Ƃ͌Ă΂��������قȂ邽�ߎ����Y���ɂ��s��ɋC��t���ĉ�����
    void FixedUpdate()
    {
        updating = true;

        //�̗͂̒l���f�[�^����ǂݏo��
        maxHP = data.GetLife();

        //����n�ʂɌ�����
        if (currentGround != null
            && currentGround.ClosestPoint(transform.position) != transform.position)
        {
            //����������ׂ��ʒu���Z�o���A
            Vector3 localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //x���𒆐S�ɂ��̈ʒu�������悤�ɉ�]������
            transform.Rotate(
                -Mathf.Atan2(localClosestPoint.z, -localClosestPoint.y)
                / Mathf.Deg2Rad, 0, 0, Space.Self);

            //�ēx����������ׂ��ʒu���Z�o���A
            localClosestPoint = transform.InverseTransformPoint(
                currentGround.ClosestPoint(transform.position));
            //z���𒆐S�ɂ��̈ʒu�������悤�ɉ�]������
            transform.Rotate(0, 0,
                Mathf.Atan2(localClosestPoint.x, -localClosestPoint.y)
                / Mathf.Deg2Rad, Space.Self);
        }

        //�o�E���h�h�~�̂��ߕύX�O��movement��ۑ�
        Vector3 prevMovement = movement;
        //�O�t���[������̈ړ��ʂ�movement�ɕϊ�
        movement = Quaternion.Inverse(prevRot)
            * ((transform.position - prevPos) / Time.deltaTime);

        //�o�E���h�h�~����
        if (Mathf.Sign(prevMovement.y) != Mathf.Sign(movement.y))
        {
            movement.y = 0;
        }

        if (view != null)
        {
            //�J�����̋p�l���K��͈͂Ɏ��߂�
            cameraAngle = Mathf.Clamp(
                cameraAngle, minCameraAngle, maxCameraAngle);
            //�J�����̋p�l���C�[�W���O
            easedCameraAngle = Mathf.Lerp(
                easedCameraAngle, cameraAngle, cameraTiltDiffuse);
            //�J�����̋������C�[�W���O
            easedCameraDistance = Mathf.Lerp(
                easedCameraDistance, cameraDistance, cameraTiltDiffuse);
            //�O�t���[������̉�]�̍��ɉ����ăJ�����̌X���p�����߂�
            cameraTiltRot =
                cameraTiltRot * (prevRot * Quaternion.Inverse(transform.rotation));
            //�J�����̌X���p������������
            cameraTiltRot = Quaternion.Slerp(
                cameraTiltRot, Quaternion.identity, cameraTiltDiffuse);
            //�܂��L�����������낷�p�x�ɃJ������������
            view.transform.localEulerAngles = new Vector3(easedCameraAngle, 0, 0);
            //�J�����̌X���p�ɉ����ăJ�������X����
            view.transform.rotation =
                cameraTiltRot * view.transform.rotation;
            //�J�����̈ʒu���J�������猩�Č�둤�ɂ���
            view.transform.localPosition =
                view.transform.localRotation * new Vector3(0, 0, -1)
                * easedCameraDistance;
            //�J�����̋������f�t�H���g�l��
            cameraDistance = defaultCameraDistance;
        }

        prevPos = transform.position;
        prevRot = transform.rotation;

        if (visual != null)
        {
            //�q�b�g�㖳�G���Ԓ��Ȃ�_��
            if ((ghostTimeFrame > 0 && Time.time % 0.1f < 0.05f)
            || !IsLive())
            {
                visual.transform.localScale = Vector3.zero;
            }
            else
            {
                visual.transform.localScale = new Vector3(1, 1, 1);
            }
            //�U�����󂯂�����Ȃ�V�F�C�N
            if (damageReactionTimeFrame > 0)
            {
                visual.transform.localPosition =
                    Vector3.Normalize(new Vector3(UnityEngine.Random.Range(1f, -1f),
                    UnityEngine.Random.Range(1f, -1f),
                    UnityEngine.Random.Range(1f, -1f))) * 0.2f; ;
            }
            else
            {
                visual.transform.localPosition = Vector3.zero;
            }
            //�L�����̌����ڂ������Ă�������֌�����
            float visualDirection = visual.transform.localEulerAngles.y;
            visual.transform.localEulerAngles = new Vector3(0,
                visualDirection
                + KX_netUtil.AngleDiff(visualDirection, direction)
                * directionTiltIntensity,
                0);
        }

        //��C��R
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
        //�d��
        movement += new Vector3(0, -data.GetGravityScale(), 0);

        //allowGroundSet�����Z�b�g
        allowGroundSet = true;
        //shield�����Z�b�g
        shield = false;

        //�X�N���v�^�u���I�u�W�F�N�g����U�����[�V�����̓��e��ǂݏo��
        UpdateAttackMotion();

        if (IsLive() && !GetGoaled())
        {
            cadaverLifeTimeFrame = maxCadaverLifeTimeFrame;

            if (IsActable())
            {
                //�����Ŋe�h���N���X�̌ŗL�X�V�������Ă�
                LiveEntityUpdate();

                //prevAttackProgress���X�V
                prevAttackProgress = GetAttackProgress();
                //�U�����[�V�����̐i�s�x�𑝉�
                attackProgress += 1 / Mathf.Max((float)attackTimeFrame, 1);
                attackProgress = Mathf.Clamp(attackProgress, 0, 1);
            }
        }
        else
        {
            //�U���������������
            attackMotionData = null;

            //�J���������o�p�̈ʒu�ɒ���
            cameraAngle = goaledCameraAngle;
            cameraDistance = goaledCameraDistance;
            //���ʂ�����
            direction = goaledDirection;

            if (cadaverLifeTimeFrame > 0)
            {
                cadaverLifeTimeFrame--;
            }
            else
            {
                if (IsPlayer())
                {
                    //�����{�^������������S�[�����̓X�e�[�W���o��A���S���͕���
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

        ghostTimeFrame = Mathf.Max(0, ghostTimeFrame - 1);
        hitBackTimeFrame = Mathf.Max(0, hitBackTimeFrame - 1);
        damageReactionTimeFrame =
            Mathf.Max(0, damageReactionTimeFrame - 1);

        //movement��velocity�ɕϊ�
        GetComponent<Rigidbody>().velocity = transform.rotation * movement;

        //�n�ʂƂ̐ڐG������s���O�Ɉ�U���n���Ă��Ȃ���Ԃɂ���
        isLanding = false;

        //�S�[�������疳�G��
        if (GetGoaled())
        {
            ghostTimeFrame = reviveGhostTimeFrame;
        }

        //���΂炭�_���[�W���󂯂Ă��Ȃ���Ή�
        if (IsLive() && IsDamageTakeable())
        {
            repairCoolTimeFrame--;
            if (repairCoolTimeFrame <= 0)
            {
                hpAmount += 0.001f;
            }
        }
        repairCoolTimeFrame = Mathf.RoundToInt(
            Mathf.Clamp(repairCoolTimeFrame, 0, maxRepairCoolTimeFrame));

        hpAmount = Mathf.Clamp(hpAmount, 0, 1);

        //�̗̓Q�[�W���X�V
        Color gaugeColor = data.GetThemeColor();
        gaugeColor.a = 1;
        lifeGauge.material.SetColor("_GaugeColor1", gaugeColor);
        lifeGauge.material.SetFloat("_FillAmount1", hpAmount);
        lifeGauge.material.SetColor("_GaugeColor2",
            KX_netUtil.DamageGaugeColor(gaugeColor));
        lifeGauge.material.SetColor("_BackGroundColor",
            KX_netUtil.GaugeBlankColor(gaugeColor));
        lifeGauge.material.SetColor("_EdgeColor",
            KX_netUtil.GaugeBlankColor(
                lifeGauge.material.GetColor("_BackGroundColor")
            ));

        if (lifeGauge.material.GetFloat("_FillAmount1")
                         <= lifeGauge.material.GetFloat("_FillAmount2"))
        {
            lifeGauge.material.SetFloat("_FillAmount2",
                lifeGauge.material.GetFloat("_FillAmount2") - 0.005f);
        }
        else
        {
            lifeGauge.material.SetFloat("_FillAmount2",
                lifeGauge.material.GetFloat("_FillAmount1"));
        }

        updating = false;
    }

    //���̃I�u�W�F�N�g���R���C�_�[�ɐG��Ă���Ԗ��t���[�����̊֐����Ă΂��i�G��Ă���R���C�_�[�������I�Ɉ����ɓ���j
    //���ӁI�@OnTriggerStay()�ƈ���č��̓��m�̏Փ˔����p�ł�
    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.GetComponent<UnLandableObject>() == null && allowGroundSet)
        {
            //����������ׂ��n�`�Ƃ��ēo�^
            currentGround = col.collider;
            // ���n����
            isLanding = true;
        }

        //�����Ŋe�h���N���X�̌ŗL�ڐG�������Ă�
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
        }

        //�����Ŋe�h���N���X�̌ŗL�ڐG�������Ă�
        LiveEntityOnHit(col);
    }

    //�e�h���N���X�̌ŗL�X�V�����i�h���N���X���ŃI�[�o�[���C�h���Ďg���j
    protected virtual void LiveEntityUpdate()
    {
    }

    //TODO:�J���I�ՂŕK�v���ۂ����f���A�s�v�Ȃ����
    //�e�h���N���X�̌ŗL�Փˏ����i�h���N���X���ŃI�[�o�[���C�h���Ďg���j
    protected virtual void LiveEntityOnHit(Collider col)
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
        attackMotionData = attackMotion;
        attackTimeFrame = Mathf.Max(attackMotionData.GetData().totalFrame, 1);
        attackProgress = 0;
    }

    //�U�����[�V��������
    protected bool IsAttacking()
    {
        return attackMotionData != null
            && (attackTimeFrame < 1 || prevAttackProgress < 1);
    }
    //�U�����[�V���������w��̍U���A�N�V�������s�Ȃ��Ă��邩
    protected bool IsAttacking(string name)
    {
        return IsAttacking() && attackMotionData.GetData().name == name;
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

    //������Ă�ł���Ԃ͒n�`�ɐG��Ă��������ɑ��������Ȃ��Ȃ�
    protected void DisAllowGroundSet()
    {
        allowGroundSet = false;
    }

    //�U�����󂯂��ۂɂ�����Ă�
    void AttackHit(AttackArea attackArea)
    {
        //�U�����󂯕t�����ԁA�������ȊO����̍U���Ȃ�
        if (IsLive() && !shield && ghostTimeFrame <= 0
            && (attackArea.GetAttacker() == null
                || attackArea.GetAttacker().GetTeamID() != teamID))
        {
            //�M�~�b�N�Ȃ�f�[�^��̐��l�����̂܂܎g��
            float damageValue = attackArea.GetData().power;
            int ghostTime = attackArea.GetData().ghostTime;
            //�L�����̍U���Ȃ�_���[�W�l�Ɩ��G���Ԃ��Z�o
            if (attackArea.GetAttacker() != null)
            {
                float attackerPower =
                    attackArea.GetAttacker().GetData().GetAttackPower();
                damageValue *= attackerPower;
                ghostTime = Mathf.RoundToInt(
                    damageValue / attackerPower * ghostTimeMul);
            }
            Damage(damageValue, ghostTime);
            //HitBack(Vector3 hitBackVec, attackArea.GetData().hitback);
        }
    }
    //�̗͂����炵�A���G���Ԃ�t�^
    void Damage(float damage, int setGhostTimeFrame)
    {
        hpAmount -= Mathf.Max(0, damage / maxHP);
        ghostTimeFrame = setGhostTimeFrame;
        repairCoolTimeFrame = maxRepairCoolTimeFrame;
        damageReactionTimeFrame = maxDamageReactionTimeFrame;
        //�_���[�W����炷
        if (IsLive())
        {
            PlayAsSE(resourcePalette.GetDamageSE());
        }
        else
        {
            PlayAsSE(resourcePalette.GetDefeatSE());
        }
    }
    //������΂����
    void HitBack(Vector3 hitBackVec, int setHitBackTimeFrame)
    {
        movement = Quaternion.Inverse(transform.rotation)
            * hitBackVec;
        hitBackTimeFrame = setHitBackTimeFrame;
    }

    //�����Ă��邩
    public bool IsLive()
    {
        return hpAmount > 0;
    }
    //�Z�ɂ�閳�G��Ԃ�
    public bool IsShield()
    {
        return /*shieldable &&*/ shield;
    }
    //�_���[�W���󂯕t�����Ԃ�
    public bool IsDamageTakeable()
    {
        return !IsShield() && ghostTimeFrame <= 0;
    }
    //�s���ł����Ԃ�
    public bool IsActable()
    {
        return hitBackTimeFrame <= 0;
    }
    //����̓v���C���[��
    public bool IsPlayer()
    {
        return GetComponent<Player>() != null;
    }
    public bool IsDestructed()
    {
        return !IsLive() && cadaverLifeTimeFrame <= 0;
    }

    //����ł���Ƃ��ɂ�����ĂԂƕ�������
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
    //�S�[���ɓ��������̏���
    void Clear()
    {
        goaled = true;
    }
    //������X�e�[�W�̔h�����Ƃ��Đݒ肳��Ă���V�[���ɖ߂�
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

    //�ݒ肳�ꂽ���[�V�����f�[�^��ǂݏo���Ď��s�i���s���͏�ɌĂԁj
    void UpdateAttackMotion()
    {
        //�ߐځA�������U���̃f�[�^�����Z�b�g
        Array.Resize(ref meleeAttackDatas, 0);
        Array.Resize(ref shotDatas, 0);
        //3D�J�[�\�������Z�b�g
        Array.Resize(ref cursors, 0);

        if (IsAttacking())
        {
            //3D�J�[�\�����擾
            if (attackMotionData.GetCursors() != null)
            {
                cursors = attackMotionData.GetCursors();
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
                        MeleeAttack(meleeAttackData, current.cursorName);
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
                        Shot(shotData, current.cursorName);
                    }
                }
            }

            //�ړ�
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
                        shield = true;
                    }
                }
            }

            //�����s����
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

            //���ʉ�
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
            current.SetData(currentData.data.attackData);
        }
        //�s�v�ȍU�����������
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

        //�e���o��
        for (int i = 0; i < shotDatas.Length; i++)
        {
            ShotAndCursorName currentData = shotDatas[i];

            //����
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
            current.SetData(currentData.data.attackData);
            current.SetProjectileData(currentData.data.projectileData,
                cursors[attackMotionData.SearchCursorIndex(currentData.cursorName)].direction);

            current.transform.parent = null;
        }
    }

    //�ߐڂ���є͈͍U��
    void MeleeAttack(AttackMotionData.MeleeAttackData attackData, string cursorName)
    {
        Array.Resize(ref meleeAttackDatas, meleeAttackDatas.Length + 1);
        meleeAttackDatas[meleeAttackDatas.Length - 1].data = attackData;
        meleeAttackDatas[meleeAttackDatas.Length - 1].cursorName = cursorName;
    }
    //�������U��
    void Shot(AttackMotionData.ShotData shotData, string cursorName)
    {
        Array.Resize(ref shotDatas, shotDatas.Length + 1);
        shotDatas[shotDatas.Length - 1].data = shotData;
        shotDatas[shotDatas.Length - 1].cursorName = cursorName;
    }
    //�������ԂȂ�ړ�
    public void Move(Vector3 setMovement)
    {
        if (IsActable())
        {
            movement = setMovement;
        }
    }
    //����LiveEntity������ʉ���炷
    public void PlayAsSE(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
    }
}