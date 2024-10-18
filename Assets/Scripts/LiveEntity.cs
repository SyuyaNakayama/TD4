using UnityEngine;
using System;
using System.Collections.Generic;

public class LiveEntity : GeoGroObject
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
        public bool postMove;
        public bool used;
    }

    [System.Serializable]
    public struct TextureSendData
    {
        public Renderer meshRenderer;
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

    static LiveEntity[] allInstances = { };
    public static LiveEntity[] GetAllInstances()
    {
        LiveEntity[] ret = new LiveEntity[allInstances.Length];
        Array.Copy(allInstances, ret, allInstances.Length);
        return ret;
    }

    const float cameraFlipMotionMultiply = 0.01f;
    const float maxCameraTiltDiffuse = 0.15f;
    const float defaultCameraDistance = 10;
    const float directionTiltIntensity = 0.5f;
    public const float minCameraAngle = 0;
    public const float maxCameraAngle = 90;
    const float goaledCameraAngle = 0;
    const float goaledCameraDistance = 3;
    const float goaledDirection = 180;
    const float ghostTimeMul = 30;
    const int reviveGhostTimeFrame = 90;
    const int maxRepairCoolTimeFrame = 600;
    const float autoRepairPower = 0.003f;
    const int maxDamageReactionTimeFrame = 10;
    const int maxCadaverLifeTimeFrame = 60;
    const int freezeCadaverLifeTimeFrame = 40;
    const int shakeCadaverLifeTimeFrame = 43;
    const int maxGoalAnimationTimeFrame = 120;
    const int maxBattery = 300;

    private GameObject gameManager;
    private MedalCounter saveMedals;

    [SerializeField]
    ResourcePalette resourcePalette;
    [SerializeField]
    GameObject visual;
    [SerializeField]
    TextureSendData[] meshes = { };
    [SerializeField]
    SpriteSendData[] sprites = { };
    [SerializeField]
    Animator[] animators = { };
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
    protected float direction;
    float visualDirection;
    Quaternion prevRot;
    Quaternion cameraTiltRot;
    float cameraTiltDiffuse;

    protected float cameraAngle = maxCameraAngle;
    float easedCameraAngle = maxCameraAngle;
    protected float cameraDistance = defaultCameraDistance;
    float easedCameraDistance = defaultCameraDistance;
    float hpAmount = 1;//?��c?��?��̗͂̊�?��?��
    public float GetHPAmount()
    {
        return hpAmount;
    }
    bool shield;//?��?��?��ꂪtrue?��̊Ԃ͋Z?��ɂ�閳?��G?��?��?��?��
    public bool GetShield()
    {
        return shield;
    }
    bool shieldable;
    public bool GetShieldable()
    {
        return shieldable;
    }
    int battery = maxBattery;
    int hitBackTimeFrame;
    int ghostTimeFrame;//?��q?��b?��g?��㖳?��G?��?��?��?��
    int repairCoolTimeFrame;
    int damageReactionTimeFrame;
    int cadaverLifeTimeFrame;
    int goalAnimationTimeFrame;
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
    string[] uniqueActDatas = { };
    protected string animationName;
    protected float animationProgress;
    float prevAnimationProgress;
    protected float animationSpeed;
    protected string facialExpressionName;
    bool updating;
    public bool GetUpdating()
    {
        return updating;
    }

    protected override void GGOAwake()
    {
        prevRot = transform.rotation;

        gameManager = GameObject.Find("/GameManager");
        saveMedals = gameManager.GetComponent<MedalCounter>();
    }

    protected override void GGOUpdate()
    {
        updating = true;

        //�S�C���X�^���X������ϐ����X�V
        List<LiveEntity> allInstancesList =
            new List<LiveEntity>(allInstances);
        allInstancesList.RemoveAll(where => !where || where == this);
        allInstances = allInstancesList.ToArray();
        Array.Resize(ref allInstances, allInstances.Length + 1);
        allInstances[allInstances.Length - 1] = this;

        if (view != null)
        {
            //?��J?��?��?��?��?��̋p?��l?��?��?��K?��?��͈͂Ɏ�?��߂�
            cameraAngle = Mathf.Clamp(
                cameraAngle, minCameraAngle, maxCameraAngle);
            //?��J?��?��?��?��?��̋p?��l?��?��?��C?��[?��W?��?��?��O
            easedCameraAngle = Mathf.Lerp(
                easedCameraAngle, cameraAngle, maxCameraTiltDiffuse);
            //?��J?��?��?��?��?��̋�?��?��?��?��?��C?��[?��W?��?��?��O
            easedCameraDistance = Mathf.Lerp(
                easedCameraDistance, cameraDistance, maxCameraTiltDiffuse);
            //?��O?��t?��?��?��[?��?��?��?��?��?��̉�]?��̍�?��ɉ�?��?��?��ăJ?��?��?��?��?��̌X?��?��?��p?��?��?��?��?��߂�
            cameraTiltRot =
                cameraTiltRot * (prevRot * Quaternion.Inverse(transform.rotation));

            //?��J?��?��?��?��?��̌X?��?��?��p?��̍�?��?��?��?��?��?��?��߂�
            float cameraTiltAmount = Mathf.Abs(
                Quaternion.Angle(cameraTiltRot, Quaternion.identity)) / 180;

            cameraTiltDiffuse = Mathf.Clamp(
                cameraTiltDiffuse + cameraTiltAmount * cameraFlipMotionMultiply,
                0, cameraTiltAmount * maxCameraTiltDiffuse);

            //?��J?��?��?��?��?��̌X?��?��?��p?��?��?��?��?��?��?��?��?��?��?��?��
            cameraTiltRot = Quaternion.Slerp(
                cameraTiltRot, Quaternion.identity,
                cameraTiltDiffuse / KX_netUtil.RangeMap(cameraTiltAmount, 1, 0, 1, 0.001f));
            //?��܂�?��L?��?��?��?��?��?��?��?��?��?��?��?��?��p?��x?��ɃJ?��?��?��?��?��?��?��?��?��?��?��?��
            view.transform.localEulerAngles = new Vector3(easedCameraAngle, 0, 0);
            //?��J?��?��?��?��?��̌X?��?��?��p?��ɉ�?��?��?��ăJ?��?��?��?��?��?��?��X?��?��?��?��
            view.transform.rotation =
                cameraTiltRot * view.transform.rotation;
            //?��J?��?��?��?��?��̈ʒu?��?��?��J?��?��?��?��?��?��?��猩?��Č�?��?��ɂ�?��?��
            view.transform.localPosition =
                view.transform.localRotation * new Vector3(0, 0, -1)
                * easedCameraDistance;
            //?��J?��?��?��?��?��̋�?��?��?��?��?��f?��t?��H?��?��?��g?��l?��?��
            cameraDistance = defaultCameraDistance;
        }

        prevRot = transform.rotation;
        dragAxis = data.GetDragAxis();

        if (hitBackTimeFrame > 0)
        {
            drag = 1;
        }
        else
        {
            drag = 0.8f;
        }
        gravityScale = data.GetGravityScale();

        //?��X?��P?��[?��?��?��?��ݒ�ɍ�?�����?��?��
        float scale = data.GetScale();
        transform.localScale = new Vector3(scale, scale, scale);

        //allowGroundSet?��?��?��?��?��Z?��b?��g
        allowGroundSet = true;
        //shield?��?��?��?��?��Z?��b?��g
        shield = false;
        //?��\?��?��?��?��?��?��Z?��b?��g
        facialExpressionName = "";
        //?��A?��j?��?��?��[?��V?��?��?��?��?��?��ʏ펞?��̂�?��̂ɂ�?��?��
        animationName = "idol";

        isAllowMove = IsActable();

        animationProgress = Mathf.Repeat(animationProgress + animationSpeed, 1);
        animationSpeed = 0;

        //?��X?��N?��?��?��v?��^?��u?��?��?��I?��u?��W?��F?��N?��g?��?��?��?��U?��?��?��?��?��[?��V?��?��?��?��?��̓�?��e?��?��ǂݏo?��?��
        UpdateAttackMotion();

        if (IsLive() && !GetGoaled())
        {
            cadaverLifeTimeFrame = maxCadaverLifeTimeFrame;
            goalAnimationTimeFrame = maxGoalAnimationTimeFrame;

            if (IsActable())
            {
                //?��?��?��?��?��Ŋe?��h?��?��?��N?��?��?��X?��̌ŗL?��X?��V?��?��?��?��?��?��?��Ă�
                LiveEntityUpdate();
            }
            else
            {
                animationName = "damage";
            }

            if (IsPlayer())
            {
                //?��G?��?��?��^?��[?��L?��[?��A?��?��?��j?��?��?��[?��{?��^?��?��?��ł�?��ł�?��E?��o
                if (Input.GetKey(KeyCode.Return)
                    || Input.GetKey("joystick button 2"))//TODO:?��N?��?��?��?��?��?��?��̃L?��[?��R?��[?��h?��?��?��?��?��j?��?��?��[?��{?��^?��?��?��̃R?��[?��h?��ɏ�?��?��?��?��?��?��?��Ă�?��?��
                {
                    Quit();
                }
            }
        }
        else
        {
            //?��U?��?��?��?��?��?��?��?��?��?��?��?��?��?��?��
            attackMotionData = null;

            //?��J?��?��?��?��?��?��?��?��?��o?��p?��̈ʒu?��ɒ�?��?��
            cameraAngle = goaledCameraAngle;
            cameraDistance = goaledCameraDistance;
            if (IsPlayer())
            {
                //?��?��?��ʂ�?��?��?��?��
                direction = goaledDirection;
            }

            if (!IsLive())
            {
                if (cadaverLifeTimeFrame > 0)
                {
                    cadaverLifeTimeFrame--;
                    animationName = "defeat";
                    animationProgress =
                        KX_netUtil.RangeMap(
                        Mathf.Clamp(cadaverLifeTimeFrame,
                        0, freezeCadaverLifeTimeFrame),
                        freezeCadaverLifeTimeFrame, 0,
                        0, 1);
                }
                else
                {
                    animationName = "dead";

                    if (IsPlayer())
                    {
                        //?��?��?��?��?��{?��^?��?��?��?��?��?��?��?��?��?��?��畜�?��
                        if (Input.GetKey(KeyCode.Space)
                            || Input.GetKey("joystick button 0")
                            || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)
                            || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.V)
                            || Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.N)
                            || Input.GetKey(KeyCode.M)
                            || Input.GetKey("joystick button 1"))
                        {
                            Revive();
                        }
                    }
                    else
                    {
                        GameObject despawnEffect =
                        Instantiate(resourcePalette.GetDespawnEffect(),
                        transform.position, transform.rotation,
                        transform.parent);
                        despawnEffect.transform.localScale = transform.localScale;
                        despawnEffect.GetComponent<ParticleSystem>().startColor =
                            data.GetThemeColor();
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                if (goalAnimationTimeFrame > 0)
                {
                    goalAnimationTimeFrame--;
                    animationName = "goal";
                    animationProgress =
                        KX_netUtil.RangeMap(
                        Mathf.Clamp(goalAnimationTimeFrame,
                        0, maxGoalAnimationTimeFrame),
                        maxGoalAnimationTimeFrame, 0,
                        0, 1);
                }
                else
                {
                    animationName = "result";
                    //?��?��?��?��?��{?��^?��?��?��?��?��?��?��?��?��?��?��玟�̃X?��e?��[?��W?��?��
                    if (Input.GetKey(KeyCode.Space)
                        || Input.GetKey("joystick button 0")
                        || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)
                        || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.V)
                        || Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.N)
                        || Input.GetKey(KeyCode.M)
                        || Input.GetKey("joystick button 1"))
                    {
                        NextStage();
                    }
                }
            }
        }

        if (visual != null)
        {
            //?��̂̃p?��[?��c?��̃g?��?��?��?��?��X?��t?��H?��[?��?��?��?��?��f?��t?��H?��?��?��g?��?��Ԃ�
            visual.transform.localScale = new Vector3(1, 1, 1);
            visual.transform.localPosition = Vector3.zero;
            //?��L?��?��?��?��?��̌�?��?��?��ڂ�?��?��?��?��?��Ă�?��?��?��?��?��?��֌�?��?��?��?��
            visualDirection += KX_netUtil.AngleDiff(visualDirection, direction)
                * directionTiltIntensity;
            visual.transform.localEulerAngles = new Vector3(0,
                visualDirection,
                0);
        }

        UpdateAnimation();

        if (visual != null)
        {
            //?��q?��b?��g?��㖳?��G?��?��?��Ԓ�?��Ȃ�_?��?��
            if ((ghostTimeFrame > 0 && Time.time % 0.1f < 0.05f)
                && IsLive() && !GetGoaled())
            {
                visual.transform.localScale = Vector3.zero;
            }
            //?��U?��?��?��?��?��?���?��?��?��?��Ȃ�V?��F?��C?��N
            if (damageReactionTimeFrame > 0
                || (!IsLive() && cadaverLifeTimeFrame > shakeCadaverLifeTimeFrame))
            {
                visual.transform.localPosition +=
                    Vector3.Normalize(new Vector3(UnityEngine.Random.Range(1f, -1f),
                    UnityEngine.Random.Range(1f, -1f),
                    UnityEngine.Random.Range(1f, -1f))) * 0.2f; ;
            }
        }

        //prevAttackProgress?��?��?��X?��V
        prevAttackProgress = GetAttackProgress();
        //?��U?��?��?��?��?��[?��V?��?��?��?��?��̐i?��s?��x?��?��?��
        attackProgress += 1 / Mathf.Max((float)attackTimeFrame, 1);
        attackProgress = Mathf.Clamp(attackProgress, 0, 1);

        ghostTimeFrame = Mathf.Max(0, ghostTimeFrame - 1);
        hitBackTimeFrame = Mathf.Max(0, hitBackTimeFrame - 1);
        damageReactionTimeFrame =
            Mathf.Max(0, damageReactionTimeFrame - 1);

        //?��S?��[?��?��?��?��?��?��?��疳?��G?��?��
        if (GetGoaled())
        {
            ghostTimeFrame = reviveGhostTimeFrame;
        }

        //?��?��?��G?��Z?��?��?��̓o?��b?��e?��?��?��[?��?��?��?��?��?��
        //?��?��?��?��؂�?��?��?��?��?��Ɖ񕜂�?��؂�?��?��?��?��?��ɖ�?��G?��ɂȂ�邩?��ۂ�?��̃t?��?��?��O?��?��؂�ւ�?��?��
        if (IsShield())
        {
            battery--;
            if (battery <= 0)
            {
                shieldable = false;
            }
        }
        else
        {
            battery++;
            if (battery >= maxBattery)
            {
                shieldable = true;
            }
        }
        battery = Mathf.Clamp(battery, 0, maxBattery);

        //?��?��?��΂炭?��_?��?��?��[?��W?��?��?��?��Ă�?��Ȃ�?��?��Ή�
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

    protected override void GGOOnCollisionStay(Collision col)
    {
        OnHit(col.collider);
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.GetComponent<Goal>() != null && IsPlayer())
        {
            Clear();
            saveMedals.Save();
        }

        OnHit(col);
    }

    //OnCollisionStay?��?��OnTriggerStay?��?��?��?��Z?��߂ɂ�?��?��?��֐�
    void OnHit(Collider col)
    {
        if (col.gameObject.GetComponent<AttackArea>() != null)
        {
            AttackHit(col.gameObject.GetComponent<AttackArea>());
        }

        //?��?��?��?��?��Ŋe?��h?��?��?��N?��?��?��X?��̌ŗL?��ڐG?��?��?��?��?��?��?��Ă�
        LiveEntityOnHit(col);
    }

    //?��e?��h?��?��?��N?��?��?��X?��̌ŗL?��X?��V?��?��?��?��?��i?��h?��?��?��N?��?��?��X?��?��?��ŃI?��[?��o?��[?��?��?��C?��h?��?��?��Ďg?��?��?��j
    protected virtual void LiveEntityUpdate()
    {
    }

    //TODO:?��J?��?��?��I?��ՂŕK?��v?��?��?��ۂ�?��?��?��f?��?��?��A?��s?��v?��Ȃ�?��?��?��
    //?��e?��h?��?��?��N?��?��?��X?��̌ŗL?��Փˏ�?��?��?��i?��h?��?��?��N?��?��?��X?��?��?��ŃI?��[?��o?��[?��?��?��C?��h?��?��?��Ďg?��?��?��j
    protected virtual void LiveEntityOnHit(Collider col)
    {

    }

    //?��U?��?��?��?��?��[?��V?��?��?��?��?��Ɉڍs
    protected void SetAttackMotion(string name)
    {
        SetAttackMotion(data.SearchAttackMotion(name));
    }

    //?��U?��?��?��?��?��[?��V?��?��?��?��?��Ɉڍs
    protected void SetAttackMotion(AttackMotionData attackMotion)
    {
        attackMotionData = attackMotion;
        attackTimeFrame = Mathf.Max(attackMotionData.GetData().totalFrame, 1);
        attackProgress = 0;
    }

    //?��U?��?��?��?��?��[?��V?��?��?��?��?��?��?��?��
    protected bool IsAttacking()
    {
        return attackMotionData != null
            && (attackTimeFrame < 1 || prevAttackProgress < 1);
    }
    //?��U?��?��?��?��?��[?��V?��?��?��?��?��?��?��?��?��w?��?��̍U?��?��?��A?��N?��V?��?��?��?��?��?��?��s?��Ȃ�?��Ă�?��邩
    protected bool IsAttacking(string name)
    {
        return IsAttacking() && attackMotionData.name == name;
    }
    //attackProgress?��?��?��w?��?��̃L?��[?��|?��C?��?��?��g?��?��ʉ߂�?��?��?��?��
    protected bool IsHitKeyPoint(float keyPoint)
    {
        return KX_netUtil.IsIntoRange(
            keyPoint, prevAttackProgress, GetAttackProgress(),
            false, true);
    }
    //attackProgress?��?��?��w?��?��͈͓̔�?��A?��?��?��?��?��?��?��͂�?��͈̔͂�1?��t?��?��?��[?��?��?��?��?��Œʉ߂�?��?��?��?��
    protected bool IsHitKeyPoint(Vector2 keyPoint)
    {
        return KX_netUtil.IsCrossingRange(
            prevAttackProgress, GetAttackProgress(),
            keyPoint.x, keyPoint.y,
            false, false);
    }

    //?��?��?��?��?��?��Ă�ł�?��?��Ԃ͒n?��`?��ɐG?��?��Ă�?��?��?��?��?��?��?��ɑ�?��?��?��?��?��?��?��Ȃ�?��Ȃ�
    protected void DisAllowGroundSet()
    {
        allowGroundSet = false;
    }

    //?��U?��?��?��?��?��?���?��ۂɂ�?��?��?��?��Ă�
    void AttackHit(AttackArea attackArea)
    {
        LiveEntity attacker = attackArea.GetAttacker();

        //?��U?��?��?��?��?��?��t?��?��?��?��?��ԁA?��?��?���?��?��?��ȊO?��?��?��?��̍U?��?��?��Ȃ�
        if (IsLive() && !IsShield() && ghostTimeFrame <= 0
            && (attacker == null
                || attacker.GetTeamID() != teamID))
        {
            //?��M?��~?��b?��N?��Ȃ�f?��[?��^?��?��̐�?��l?��?��?��?��?��̂܂܎g?��?��
            float damageValue = attackArea.GetData().power;
            int ghostTime = attackArea.GetData().ghostTime;
            //?��L?��?��?��?��?��̍U?��?��?��Ȃ�_?��?��?��[?��W?��l?��Ɩ�?��G?��?��?��Ԃ�?��Z?��o
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

            Vector3 hitBackVec = Vector3.zero;
            if (attackArea.GetData().vectorBlow)
            {
                hitBackVec =
                    (attackArea.transform.rotation
                    * attackArea.GetBlowVec()).normalized
                    * attackArea.GetData().blowForce;
            }
            else
            {
                hitBackVec =
                    (transform.position
                    - attackArea.transform.TransformPoint(
                    attackArea.GetBlowVec())).normalized
                    * attackArea.GetData().blowForce;
            }
            if (!data.GetHeavy())
            {
                HitBack(hitBackVec, attackArea.GetData().hitback);
            }
        }
    }
    //?��̗͂�?��?��?��炵?��A?��?��?��G?��?��?��Ԃ�t?��^
    void Damage(float damage, int setGhostTimeFrame)
    {
        hpAmount -= Mathf.Max(0, damage / data.GetLife());
        ghostTimeFrame = setGhostTimeFrame;
        repairCoolTimeFrame = maxRepairCoolTimeFrame;
        damageReactionTimeFrame = maxDamageReactionTimeFrame;
        //?��_?��?��?��[?��W?��?��?��?��炷
        if (IsLive())
        {
            PlayAsSE(resourcePalette.GetDamageSE());
        }
        else
        {
            PlayAsSE(resourcePalette.GetDefeatSE());
            if (!IsPlayer() && GetData().GetWeaponedAttackMotionName() != "")
            {
                //?��A?��C?��e?��?��?��?���
                CharaChip current =
                    Instantiate(resourcePalette.GetCharaChip().gameObject,
                    transform.position, transform.rotation, transform)
                    .GetComponent<CharaChip>();
                current.SetData(GetData());
                current.gameObject.transform.parent = transform.parent;
            }
        }

        GameObject damageEffect =
                Instantiate(resourcePalette.GetDamageEffect(),
                transform.position, transform.rotation, transform);
        damageEffect.transform.localScale = new Vector3(1, 1, 1);
        damageEffect.GetComponent<ParticleSystem>().startColor = data.GetThemeColor();
    }
    //?��?��?��?��?��?��΂�?��?��?��
    void HitBack(Vector3 hitBackVec, int setHitBackTimeFrame)
    {
        movement = Quaternion.Inverse(transform.rotation)
            * hitBackVec;
        hitBackTimeFrame = setHitBackTimeFrame;
        attackMotionData = null;
    }

    //?��?��?��?��?��Ă�?��邩
    public bool IsLive()
    {
        return hpAmount > 0;
    }
    //?��Z?��ɂ�閳?��G?��?��Ԃ�
    public bool IsShield()
    {
        return shieldable && shield;
    }
    //?��_?��?��?��[?��W?��?��?��?��t?��?��?��?��?��Ԃ�
    public bool IsDamageTakeable()
    {
        return !IsShield() && ghostTimeFrame <= 0;
    }
    //?��s?��?��?��ł�?��?��?��Ԃ�
    public bool IsActable()
    {
        return hitBackTimeFrame <= 0;
    }
    //?��?��?��?��̓v?��?��?��C?��?��?��[?��?��
    public bool IsPlayer()
    {
        return GetComponent<Player>() != null;
    }
    public bool IsDestructed()
    {
        return !IsLive() && cadaverLifeTimeFrame <= 0;
    }
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
    public float GetBatteryAmount()
    {
        return (float)battery / maxBattery;
    }

    //?��?��?��?��ł�?��?��Ƃ�?��ɂ�?��?��?��?��ĂԂƕ�?��?��?��?��?��?��
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
    //?��S?��[?��?��?��ɓ�?��?��?��?��?��?��?��̏�?��?��
    void Clear()
    {
        goaled = true;
    }
    //?��?��?��?��?��?��X?��e?��[?��W?��̎�?��ɐݒ肳?��?��Ă�?��?��X?��e?��[?��W?��֐i?��?��
    void NextStage()
    {
        if (StageManager.GetCurrent().gameObject.activeInHierarchy)
        {
            SceneTransition.ChangeScene(StageManager.GetCurrent().GetNextStageName());
            return;
        }
    }
    //?��?��?��?��?��?��X?��e?��[?��W?��̔h?��?��?��?��?��Ƃ�?��Đݒ肳?��?��Ă�?��?��V?��[?��?��?��ɖ߂�
    void Quit()
    {
        if (StageManager.GetCurrent().gameObject.activeInHierarchy)
        {
            SceneTransition.ChangeScene(StageManager.GetCurrent().GetQuitSceneName());
        }
    }

    //?��ݒ肳?��ꂽ?��U?��?��?��?��?��[?��V?��?��?��?��?��f?��[?��^?��?��ǂݏo?��?��?��Ď�?��s?��i?��?��?��s?��?��?��͏�ɌĂԁj
    void UpdateAttackMotion()
    {
        //3D?��J?��[?��\?��?��?��?��?��?��?��Z?��b?��g
        Array.Resize(ref cursors, 0);

        //?��ŗL?��?��?��?��|?��C?��?��?��g?��?��?��?��?��Z?��b?��g
        Array.Resize(ref uniqueActDatas, 0);

        if (IsAttacking())
        {
            //3D?��J?��[?��\?��?��?��?��?��擾
            if (attackMotionData.GetCursors() != null)
            {
                cursors = attackMotionData.GetCursors();
            }

            //?��ߐڍU?��?��
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

            //?��?��?��?��?��?��?��U?��?��
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
                        Shot(shotData, current.cursorName, current.postMove);
                    }
                }
            }

            //?��ړ�
            if (attackMotionData.GetData().moveKeys != null)
            {
                Vector3 savedMovement = movement;
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
                        movement = Vector3.zero;
                        break;
                    }
                }

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

                        replaceVector += Quaternion.Euler(new Vector3(0, direction, 0))
                            * current.moveVec * (key1 - key0) / Time.deltaTime;
                    }
                }

                movement += replaceVector;
            }

            //?��ŗL?��?��?��?��|?��C?��?��?��g
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

            //?��?��?��G?��?��?��?��
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

            //?��?��?��?��?��s?���?��?��
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

            //?��?��?��ʉ�
            if (attackMotionData.GetData().seKeys != null)
            {
                for (int i = 0; i < attackMotionData.GetData().
                    seKeys.Length; i++)
                {
                    AttackMotionData.SEKey current =
                        attackMotionData.GetData().seKeys[i];
                    if (IsHitKeyPoint(current.keyFrame))
                    {
                        PlayAsSE(current.se);
                    }
                }
            }

            //?��A?��j?��?��?��[?��V?��?��?��?��
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
                        if (!current.useOriginalAnimTime)
                        {
                            animationProgress =
                                KX_netUtil.RangeMap(GetAttackProgress(),
                                current.keyFrame.x, current.keyFrame.y,
                                0, 1);
                        }
                    }
                }
            }
        }

        //?��U?��?��?��?��?��?��?��?��o?��?��
        //?��܂�?��͋ߐڍU?��?��?��̃f?��[?��^?��Ɠ�?��?��?��?��?��?��?��?��?��̈�?��p?��?��
        Array.Resize(ref attackAreas, meleeAttackDatas.Length);
        //?��̈�?��?��̍U?��?��?��?��?��?��ɋߐڍU?��?��?��̃f?��[?��^?��?��?��?��
        for (int i = 0; i < attackAreas.Length; i++)
        {
            MeleeAttackAndCursorName currentData = meleeAttackDatas[i];

            //?��?��?��?��?��?��ΐ�?��?��
            if (attackAreas[i] == null)
            {
                attackAreas[i] =
                    Instantiate(resourcePalette.GetAttackArea().gameObject,
                    transform.position,
                    transform.rotation * Quaternion.Euler(0, direction, 0),
                    transform)
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
            current.Lock();
        }
        //?��s?��v?��ȍU?��?��?��?��?��?��?��?��?��?��?��
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

        //?��e?��?��?��o?��?��
        for (int i = 0; i < shotDatas.Length; i++)
        {
            ShotAndCursorName currentData = shotDatas[i];

            if (currentData.postMove)
            {
                //postMove?��?��true?��Ȃ�1?��t?��?��?��[?��?��?��x?��点?��?��
                shotDatas[i].postMove = false;
            }
            else
            {
                if (attackMotionData)
                {
                    AttackMotionData.Cursor currentCursor =
                        cursors[attackMotionData.SearchCursorIndex(currentData.cursorName)];

                    //?��?��?��?��
                    Projectile current =
                            Instantiate(resourcePalette.GetProjectile().gameObject,
                            transform.position,
                            transform.rotation * Quaternion.Euler(0, direction, 0),
                            transform)
                            .GetComponent<Projectile>();

                    current.transform.parent = gameObject.transform;

                    float projectileScale = currentData.data.scale;
                    current.transform.localScale =
                        new Vector3(projectileScale, projectileScale, projectileScale);
                    current.transform.localPosition =
                        Quaternion.Euler(new Vector3(0, direction, 0))
                        * currentCursor.pos;
                    current.transform.localRotation =
                        Quaternion.Euler(new Vector3(0, direction, 0));
                    current.SetAttacker(this);
                    current.SetData(currentData.data.attackData,
                        currentCursor.direction);
                    current.SetProjectileData(currentData.data.projectileData,
                        currentCursor.direction);
                    current.Lock();

                    current.transform.parent = null;
                }

                shotDatas[i].used = true;
            }
        }

        //?��ߐځA?��?��?��?��?��?��?��U?��?��?��̃f?��[?��^?��?��?��?��g?��p?��ς݂̗v?��f?��?��?��?��?��?��
        Array.Resize(ref meleeAttackDatas, 0);

        List<ShotAndCursorName> shotDataList =
            new List<ShotAndCursorName>(shotDatas);
        shotDataList.RemoveAll(where => where.used);
        shotDatas = shotDataList.ToArray();
    }

    //?��ݒ肳?��ꂽ?��A?��j?��?��?��[?��V?��?��?��?��?��f?��[?��^?��?��ǂݏo?��?��?��Ď�?��s?��i?��?��?��s?��?��?��͏�ɌĂԁj
    void UpdateAnimation()
    {
        //?��܂�?��p?��[?��c?��̃g?��?��?��?��?��X?��t?��H?��[?��?��?��?��?��f?��t?��H?��?��?��g?��l?��ɑ�?��?��?��?��
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
        //?��?��?��݂̏�Ԃɂ�?��?��?��?��?��A?��j?��?��?��[?��V?��?��?��?��?��?��?��擾
        CharaData.Animation animationData =
            data.SearchAnimation(animationName);
        //?��g?��?��?��?��?��X?��t?��H?��[?��?��?��A?��j?��?��?��[?��V?��?��?��?��?��?��K?��p
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

                    if (!tAnimData.ignoreTransform.position)
                    {
                        current.localPosition = Vector3.Lerp(
                            tAnimData.startTransform.position,
                            tAnimData.endTransform.position,
                            animationPartProgress);
                    }

                    if (!tAnimData.ignoreTransform.rotation)
                    {
                        Quaternion rotate;
                        if (tAnimData.lerpAsEuler)
                        {
                            rotate = Quaternion.Euler(Vector3.Lerp(
                                tAnimData.startTransform.eulerAngles,
                                tAnimData.endTransform.eulerAngles,
                                animationPartProgress));
                        }
                        else
                        {
                            rotate = Quaternion.Slerp(
                                Quaternion.Euler(tAnimData.startTransform.eulerAngles),
                                Quaternion.Euler(tAnimData.endTransform.eulerAngles),
                                animationPartProgress);
                        }

                        if (current == visual.transform)
                        {
                            current.Rotate(rotate.eulerAngles,
                                Space.Self);
                        }
                        else
                        {
                            current.localRotation = rotate;
                        }
                    }

                    if (!tAnimData.ignoreTransform.scale)
                    {
                        current.localScale = Vector3.Lerp(
                        tAnimData.startTransform.scale,
                        tAnimData.endTransform.scale,
                        animationPartProgress);
                    }
                }
            }
            if (animationData.totalFrame > 0)
            {
                animationSpeed = 1f / animationData.totalFrame;
            }
        }

        //?��X?��L?��?��?��A?��j?��?��?��[?��V?��?��?��?��?��?��K?��p
        if (animationData.rigAnimationKeys != null)
        {
            for (int i = 0; i < animationData.rigAnimationKeys.Length; i++)
            {
                CharaData.RigAnimationKey rAnimData =
                    animationData.rigAnimationKeys[i];
                if (KX_netUtil.IsIntoRange(
                    animationProgress,
                    rAnimData.keyFrame.x, rAnimData.keyFrame.y,
                    false, false))
                {
                    Animator current = animators[rAnimData.animatorIndex];
                    float animationPartProgress =
                        KX_netUtil.RangeMap(animationProgress,
                        rAnimData.keyFrame.x, rAnimData.keyFrame.y,
                        0, 1);

                    current.SetInteger("statusID", rAnimData.rigAnimationID);
                    current.Play(current.GetNextAnimatorStateInfo(0).nameHash,
                    0, animationPartProgress);
                }
            }
        }

        //?��\?��?��?��K?��p
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

        //?��?��?��ʉ�
        if (animationData.seKeys != null)
        {
            for (int i = 0; i < animationData.
                seKeys.Length; i++)
            {
                AttackMotionData.SEKey current =
                    animationData.seKeys[i];

                float shiftedprevAnimationProgress = prevAnimationProgress;
                if (animationProgress < prevAnimationProgress)
                {
                    shiftedprevAnimationProgress -= 1;
                }

                if (KX_netUtil.IsIntoRange(
                    current.keyFrame,
                    shiftedprevAnimationProgress, animationProgress,
                    false, false))
                {
                    PlayAsSE(current.se);
                }
            }
        }

        prevAnimationProgress = animationProgress;

        //?��f?��t?��H?��?��?��g?��̃e?��N?��X?��`?��?��?��?��?��?��?��f?��?��?��ɓ\?��?��
        for (int i = 0; i < meshes.Length; i++)
        {
            TextureSendData current = meshes[i];
            current.meshRenderer.materials[current.index].
                SetTexture(current.propertyName, data.GetDefaultTexture(i));
        }
        //?��f?��t?��H?��?��?��g?��̃X?��v?��?��?��C?��g?��?��?��X?��v?��?��?��C?��g?��?��?��?��?��_?��?��?��[?��ɓ\?��?��
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

        //?��?��?��݂̏�Ԃɂ�?��?��?��?��?��\?��?��?��?��擾
        CharaData.FacialExpression facialData =
            data.SearchFacialExpression(facialExpressionName);
        //?��\?��?��̃e?��N?��X?��`?��?��?��?��?��?��?��f?��?��?��ɓ\?��?��
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
        //?��\?��?��̃X?��v?��?��?��C?��g?��?��?��X?��v?��?��?��C?��g?��?��?��?��?��_?��?��?��[?��ɓ\?��?��
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
    }

    //?��ߐڂ�?��?��є͈͍U?��?��
    void MeleeAttack(AttackMotionData.MeleeAttackData attackData, string cursorName)
    {
        Array.Resize(ref meleeAttackDatas, meleeAttackDatas.Length + 1);
        meleeAttackDatas[meleeAttackDatas.Length - 1].data = attackData;
        meleeAttackDatas[meleeAttackDatas.Length - 1].cursorName = cursorName;
    }
    //?��?��?��?��?��?��?��U?��?��
    void Shot(AttackMotionData.ShotData shotData, string cursorName, bool postMove)
    {
        Array.Resize(ref shotDatas, shotDatas.Length + 1);
        shotDatas[shotDatas.Length - 1].data = shotData;
        shotDatas[shotDatas.Length - 1].cursorName = cursorName;
        shotDatas[shotDatas.Length - 1].postMove = postMove;
        shotDatas[shotDatas.Length - 1].used = false;
    }
    //?��?��?��?��LiveEntity?��?��?��?��?��?��ʉ�?��?��炷
    public void PlayAsSE(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
    }

    public static void Spawn(LiveEntity liveEntity,
        Vector3 worldPos, Quaternion worldRot, string setTeamID)
    {
        LiveEntity current =
            Instantiate(liveEntity.gameObject, worldPos, worldRot)
            .GetComponent<LiveEntity>();
        current.teamID = setTeamID;
    }
}