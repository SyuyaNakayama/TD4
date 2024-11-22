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

    public const float defaultCameraDistance = 10;
    public const float MinCameraAngle = 0;
    public const float MaxCameraAngle = 90;
    const float goaledCameraAngle = 0;
    const float goaledCameraDistance = 3;
    const float goaledDirection = 180;
    const float ghostTimeMul = 30;
    const int reviveGhostTimeFrame = 90;
    const int maxRepairCoolTimeFrame = 600;
    const float autoRepairPower = 0.003f;
    const int maxDamageReactionTimeFrame = 10;
    public const int MaxCadaverLifeTimeFrame = 60;
    public const int DeadIndicateCadaverLifeTimeFrame = 43;
    public const int maxGoalAnimationTimeFrame = 120;
    const int maxBattery = 300;

    private GameObject gameManager;
    private MedalCounter saveMedals;

    [SerializeField]
    ResourcePalette resourcePalette;
    public ResourcePalette GetResourcePalette()
    {
        return resourcePalette;
    }
    [SerializeField]
    CharaDataLib lib;
    public CharaDataLib GetLib()
    {
        return lib;
    }
    [SerializeField]
    ControlMapPlayPart controlMap;
    public ControlMapPlayPart GetControlMap()
    {
        return controlMap;
    }
    [SerializeField]
    Camera view;
    public Camera GetView()
    {
        return view;
    }
    [SerializeField]
    string teamID;
    public string GetTeamID()
    {
        return teamID;
    }
    [SerializeField]
    bool userControl;
    public bool GetUserControl()
    {
        return userControl;
    }
    [SerializeField]
    CassetteSlot slot;
    public CassetteSlot GetSlot()
    {
        return slot;
    }
    [SerializeField]
    Menu menu;
    [SerializeField]
    Menu quitMenu;
    [SerializeField]
    CMBSlider nutralCameraTiltSlider;
    [SerializeField]
    CMBSlider cameraFlipMotionMultiplySlider;
    [SerializeField]
    CMBSlider cameraTiltSpeedSlider;


    bool prevMenuInput;

    float direction;
    public float GetDirection()
    {
        return direction;
    }
    public Quaternion GetDirectionQuat()
    {
        return Quaternion.Euler(0, direction, 0);
    }
    Quaternion prevRot;
    Quaternion cameraTiltRot;
    float cameraTiltDiffuse;

    protected float cameraAngle;
    float easedCameraAngle;
    protected float cameraDistance = defaultCameraDistance;
    float easedCameraDistance = defaultCameraDistance;

    float life = 1;//�c��̗͂̊���
    public float GetLife()
    {
        return life;
    }
    bool shield;//���ꂪtrue�̊Ԃ͋Z�ɂ�閳�G����
    public bool GetShield()
    {
        return shield;
    }
    public void Shield()
    {
        shield = true;
    }
    bool shieldable;
    public bool GetShieldable()
    {
        return shieldable;
    }
    int battery = maxBattery;
    int hitbackTimeFrame;
    int ghostTimeFrame;//�q�b�g�㖳�G����
    int repairCoolTimeFrame;
    int damageReactionTimeFrame;
    public int GetDamageReactionTimeFrame()
    {
        return damageReactionTimeFrame;
    }
    int cadaverLifeTimeFrame;
    public int GetCadaverLifeTimeFrame()
    {
        return cadaverLifeTimeFrame;
    }
    int goalAnimationTimeFrame;
    public int GetGoalAnimationTimeFrame()
    {
        return goalAnimationTimeFrame;
    }
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
    string[] uniqueActDatas = { };
    bool updating;
    public bool GetUpdating()
    {
        return updating;
    }
    bool isAllowCassetteUpdate;
    public bool GetIsAllowCassetteUpdate()
    {
        return isAllowCassetteUpdate;
    }
    GameObject cassetteObj;
    int tempCassetteIndex;
    int cassetteIndex = -1;
    public int GetCassetteIndex()
    {
        return cassetteIndex;
    }
    Item[] touchedItems = { };
    bool allowedItemEffect;
    public bool GetAllowedItemEffect()
    {
        return allowedItemEffect;
    }
    bool initAble = true;

    protected override void GGOAwake()
    {
        initAble = true;
    }

    void Start()
    {
        Init(transform.rotation, userControl, teamID,
            GetSlot().GetInventoryCharaID(),
            GetSlot().GetTeam(), tempCassetteIndex);

        gameManager = GameObject.Find("/GameManager");
        saveMedals = gameManager.GetComponent<MedalCounter>();

        cameraAngle = nutralCameraTiltSlider.GetScaledOutputValue();
        easedCameraAngle = nutralCameraTiltSlider.GetScaledOutputValue();
    }

    protected override void GGOUpdate()
    {
        updating = true;

        List<LiveEntity> allInstancesList =
            new List<LiveEntity>(allInstances);
        allInstancesList.RemoveAll(where => !where || where == this);
        allInstances = allInstancesList.ToArray();
        Array.Resize(ref allInstances, allInstances.Length + 1);
        allInstances[allInstances.Length - 1] = this;

        //���삵�Ă���L�����N�^�[�̃J�����ƃI�[�f�B�I���X�i�[���I���ɂ���
        view.enabled =
        view.GetComponent<AudioListener>().enabled = userControl;

        List<Item> touchedItemList = new List<Item>(touchedItems);
        touchedItemList.Remove(null);
        touchedItems = touchedItemList.ToArray();

        allowedItemEffect = true;
        for (int i = 0; i < touchedItems.Length; i++)
        {
            touchedItems[i].Activation(this);
        }
        Array.Resize(ref touchedItems, 0);
        allowedItemEffect = false;

        UpdateView();

        dragAxis = GetCassetteData().GetDragAxis();
        if (hitbackTimeFrame > 0)
        {
            drag = 0;
        }
        else
        {
            drag = 0.3f;
        }
        gravityVec = new Vector3(0, -GetCassetteData().GetGravityScale(), 0);

        float scale = GetCassetteData().GetScale();
        transform.localScale = new Vector3(scale, scale, scale);

        allowGroundSet = true;
        shield = false;

        isAllowMove = IsActable();

        //�g���Ă��Ȃ��L�����N�^�[�I�u�W�F�N�g������
        for (int i = 0; i < transform.childCount; i++)
        {
            CharacterCassette currentCassette
                = transform.GetChild(i).gameObject.
                GetComponent<CharacterCassette>();
            if (currentCassette && currentCassette != GetCassette())
            {
                Destroy(currentCassette.gameObject);
            }
        }

        //�L�����N�^�[�I�u�W�F�N�g�𐶐�
        if (GetSlot().GetEnabled(tempCassetteIndex) && (!cassetteObj
            || (cassetteObj.GetComponent<CharacterCassette>().GetData().name
            != GetSlot().GetTeamCharaID(tempCassetteIndex)
            && cassetteObj.GetComponent<CharacterCassette>().GetData().name
            != lib.FindCharacterCassette(GetSlot().GetTeamCharaID(tempCassetteIndex)).GetData().name)))
        {
            cassetteObj = Instantiate(lib.FindCharacterCassette(
                GetSlot().GetTeamCharaID(tempCassetteIndex)).gameObject,
                transform.position, transform.rotation, transform);
        }

        //�L�����N�^�[�̍X�V����
        if (GetCassette())
        {
            cassetteObj.transform.parent = transform;
            isAllowCassetteUpdate = true;
            GetCassette().CassetteUpdate();
        }
        isAllowCassetteUpdate = false;

        if (IsLive() && !GetGoaled())
        {
            cadaverLifeTimeFrame = MaxCadaverLifeTimeFrame;
            goalAnimationTimeFrame = maxGoalAnimationTimeFrame;

            LiveEntityUpdate();

            if (userControl)
            {
                if (controlMap.GetMenuInput() && !prevMenuInput)
                {
                    menu.active = !menu.active;
                }
                if (quitMenu.GetControlMessage() == "yes")
                {
                    Quit();
                }
            }
        }
        else
        {
            cameraAngle = goaledCameraAngle;
            cameraDistance = goaledCameraDistance;
            if (userControl)
            {
                direction = goaledDirection;
            }

            if (!IsLive())
            {
                if (cadaverLifeTimeFrame > 0)
                {
                    cadaverLifeTimeFrame--;
                }
                else
                {
                    if (userControl)
                    {
                        if (GetControlMap().GetJumpInput()
                            || GetControlMap().GetWeaponInput())
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
                            GetCassetteData().GetThemeColor();
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                if (goalAnimationTimeFrame > 0)
                {
                    goalAnimationTimeFrame--;
                }
                else
                {
                    if (GetControlMap().GetJumpInput()
                        || GetControlMap().GetWeaponInput())
                    {
                        NextStage();
                    }
                }
            }
        }

        ghostTimeFrame = Mathf.Max(0, ghostTimeFrame - 1);
        hitbackTimeFrame = Mathf.Max(0, hitbackTimeFrame - 1);
        damageReactionTimeFrame =
            Mathf.Max(0, damageReactionTimeFrame - 1);

        if (GetGoaled())
        {
            ghostTimeFrame = reviveGhostTimeFrame;
        }

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

        if (IsLive() && IsDamageTakeable())
        {
            repairCoolTimeFrame--;
            if (repairCoolTimeFrame <= 0)
            {
                life += autoRepairPower;
            }
        }
        repairCoolTimeFrame = Mathf.RoundToInt(
            Mathf.Clamp(repairCoolTimeFrame, 0, maxRepairCoolTimeFrame));

        life = Mathf.Clamp(life, 0, 1);

        //�g���K�[���͗p�̕ϐ����X�V
        prevMenuInput = controlMap.GetMenuInput();

        updating = false;
    }

    protected override void GGOOnCollisionStay(Collision col)
    {
        OnHit(col.collider);
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.GetComponent<Goal>() != null && userControl)
        {
            Clear();
            saveMedals.Save();
        }

        OnHit(col);
    }

    //OnCollisionStay??��?��??��?��OnTriggerStay??��?��??��?��??��?��??��?��Z??��?��߂ɂ�??��?��??��?��??��?��֐�
    void OnHit(Collider col)
    {
        if (col.gameObject.GetComponent<AttackArea>() != null)
        {
            AttackHit(col.gameObject.GetComponent<AttackArea>());
        }

        //??��?��??��?��??��?��??��?��??��?��Ŋe??��?��h??��?��??��?��??��?��N??��?��??��?��??��?��X??��?��̌ŗL??��?��ڐG??��?��??��?��??��?��??��?��??��?��??��?��??��?��Ă�
        LiveEntityOnHit(col);
    }

    //??��?��e??��?��h??��?��??��?��??��?��N??��?��??��?��??��?��X??��?��̌ŗL??��?��X??��?��V??��?��??��?��??��?��??��?��??��?��i??��?��h??��?��??��?��??��?��N??��?��??��?��??��?��X??��?��??��?��??��?��ŃI??��?��[??��?��o??��?��[??��?��??��?��??��?��C??��?��h??��?��??��?��??��?��Ďg??��?��??��?��??��?��j
    protected virtual void LiveEntityUpdate()
    {
    }

    //TODO:??��?��J??��?��??��?��??��?��I??��?��ՂŕK??��?��v??��?��??��?��??��?��ۂ�??��?��??��?��??��?��f??��?��??��?��??��?��A??��?��s??��?��v??��?��Ȃ�??��?��??��?��??��?��
    //??��?��e??��?��h??��?��??��?��??��?��N??��?��??��?��??��?��X??��?��̌ŗL??��?��Փˏ�??��?��??��?��??��?��i??��?��h??��?��??��?��??��?��N??��?��??��?��??��?��X??��?��??��?��??��?��ŃI??��?��[??��?��o??��?��[??��?��??��?��??��?��C??��?��h??��?��??��?��??��?��Ďg??��?��??��?��??��?��j

    protected virtual void LiveEntityOnHit(Collider col)
    {
        Item item = col.GetComponent<Item>();
        if (item)
        {
            //?��?��?��?��?��?��?��?��?��?��?��?��?��?��?��?�?接触?��?��?��?��?��?��?��?��?��?��?��?��?��?��?��A?��?��?��C?��?��?��e?��?��?��?��?��?��?��?��?��?��?��?��z?��?��?��?��?��?��?��追会ｿ?��
            //(?��?��?��s?��?��?��?��?��?��?��?��?��?��ア?��?��?��C?��?��?��e?��?��?��?��?��?��?��?��?��?��?��?��?��?��?��謫?��?��?��?��?��?��?��?��?��?��?��?��?��`?��?��?��[?��?��?��g?��?��?��?��?��?��h?��?��?��~?��?��?��?��?��?��?��?��?��驍ｽ?��?��?��?��ゑ?��?��?��?��?��?��よ�??��?��?��?��措?��?��?��u?��?��?��?��?��?��?��?��?��?��?��?��?��?��?��?��?��?��?�?ゑｿ?��?��?��?��?��ゑ?��?��)
            Array.Resize(ref touchedItems, touchedItems.Length + 1);
            touchedItems[touchedItems.Length - 1] = item;
        }
    }

    public CharacterCassette GetCassette()
    {
        if (!cassetteObj)
        {
            return null;
        }
        return cassetteObj.GetComponent<CharacterCassette>();
    }
    public CharaData GetCassetteData()
    {
        if (!GetCassette())
        {
            return new CharaData();
        }
        return GetCassette().GetData();
    }

    //??��?��??��?��??��?��??��?��??��?��??��?��Ă�ł�??��?��??��?��Ԃ͒n??��?��`??��?��ɐG??��?��??��?��Ă�??��?��??��?��??��?��??��?��??��?��??��?��??��?��ɑ�??��?��??��?��??��?��??��?��??��?��??��?��??��?��Ȃ�??��?��Ȃ�
    public void DisAllowGroundSet()
    {
        if (isAllowCassetteUpdate)
        {
            allowGroundSet = false;
        }
    }

    //??��?��U??��?��??��?��??��?��??��?��??��?��??��?��?��??��?��ۂɂ�??��?��??��?��??��?��??��?��Ă�
    void AttackHit(AttackArea attackArea)
    {
        LiveEntity attacker = attackArea.GetAttacker();

        //??��?��U??��?��??��?��??��?��??��?��??��?��??��?��t??��?��??��?��??��?��??��?��??��?��ԁA??��?��??��?��??��?���??��?��??��?��??��?��ȊO??��?��??��?��??��?��??��?��̍U??��?��??��?��??��?��Ȃ�
        if (IsLive() && !IsShield() && ghostTimeFrame <= 0
            && (attacker == null
                || attacker.GetTeamID() != teamID))
        {
            //??��?��M??��?��~??��?��b??��?��N??��?��Ȃ�f??��?��[??��?��^??��?��??��?��̐�??��?��l??��?��??��?��??��?��??��?��??��?��̂܂܎g??��?��??��?��
            float damageValue = attackArea.GetData().power;
            int ghostTime = attackArea.GetData().ghostTime;
            //??��?��L??��?��??��?��??��?��??��?��??��?��̍U??��?��??��?��??��?��Ȃ�_??��?��??��?��??��?��[??��?��W??��?��l??��?��Ɩ�??��?��G??��?��??��?��??��?��Ԃ�??��?��Z??��?��o
            if (attacker != null)
            {
                float attackerPower =
                    attacker.GetCassetteData().GetAttackPower();
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
            if (!GetCassetteData().GetHeavy())
            {
                HitBack(hitBackVec, attackArea.GetData().hitback);
            }
        }
    }
    //�_���[�W����
    void Damage(float damage, int setGhostTimeFrame)
    {
        life -= Mathf.Max(0, damage / GetCassetteData().GetLife());
        ghostTimeFrame = setGhostTimeFrame;
        repairCoolTimeFrame = maxRepairCoolTimeFrame;
        damageReactionTimeFrame = maxDamageReactionTimeFrame;

        if (IsLive())
        {
            PlayAsSE(resourcePalette.GetDamageSE());
        }
        else
        {
            PlayAsSE(resourcePalette.GetDefeatSE());
            if (!userControl && GetCassetteData().GetWeaponedAttackMotionName() != "")
            {
                CharaChip current =
                    Instantiate(resourcePalette.GetCharaChip().gameObject,
                    transform.position, transform.rotation, transform)
                    .GetComponent<CharaChip>();
                current.SetData(GetCassetteData());
                current.gameObject.transform.parent = transform.parent;
            }
        }

        GameObject damageEffect =
                Instantiate(resourcePalette.GetDamageEffect(),
                transform.position, transform.rotation, transform);
        damageEffect.transform.localScale = new Vector3(1, 1, 1);
        damageEffect.GetComponent<ParticleSystem>().startColor = GetCassetteData().GetThemeColor();
    }
    //�q�b�g�o�b�N
    void HitBack(Vector3 hitBackVec, int setHitBackTimeFrame)
    {
        movement = Quaternion.Inverse(transform.rotation)
            * hitBackVec;
        hitbackTimeFrame = setHitBackTimeFrame;
    }

    public bool IsTouchedThisItem(Item item)
    {
        if (item == null)
        {
            return false;
        }
        for (int i = 0; i < touchedItems.Length; i++)
        {
            if (touchedItems[i] == item)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsHitBacking()
    {
        return hitbackTimeFrame > 0;
    }
    //�����Ă��邩
    public bool IsLive()
    {
        return life > 0;
    }
    public bool IsPeril()
    {
        return life <= 0.25f;
    }
    public bool IsVanished()
    {
        return !IsLive() && cadaverLifeTimeFrame <= 0;
    }
    //�Z�ɂ�閳�G��Ԃ�
    public bool IsShield()
    {
        return shieldable && shield;
    }
    public bool IsGhostTime()
    {
        return ghostTimeFrame > 0;
    }
    //�_���[�W���󂯂��Ԃ�
    public bool IsDamageTakeable()
    {
        return !IsShield() && !IsGhostTime();
    }
    //����ł����Ԃ�
    public bool IsActable()
    {
        return hitbackTimeFrame <= 0;
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

    void Init(Quaternion setRotation, bool setUserControl, string setTeamID, string[] inventoryCharaID, int[] setTeamMember, int setCassetteIndex, float setLife = 1)
    {
        if (initAble)
        {
            goaled = false;
            userControl = setUserControl;
            teamID = setTeamID;
            life = setLife;
            GetSlot().Restart(inventoryCharaID);
            transform.rotation = setRotation;
            ghostTimeFrame = reviveGhostTimeFrame;
            prevRot = setRotation;
        }
        initAble = false;
    }
    //����ł����畜��
    void Revive()
    {
        if (!IsLive())
        {
            life = 1;
            hitbackTimeFrame = 0;
            ghostTimeFrame = reviveGhostTimeFrame;
            reviveCount++;
        }
    }
    void CharacterChange(int setCassetteIndex)
    {
        setCassetteIndex = Mathf.RoundToInt(Mathf.Repeat(setCassetteIndex, CassetteSlot.teamNum));
    }
    //�S�[���G���A�ɐG�ꂽ���̏���
    void Clear()
    {
        goaled = true;
    }
    //���̃X�e�[�W�Ƃ��Đݒ肳��Ă���V�[���֔��
    void NextStage()
    {
        if (StageManager.GetCurrent().gameObject.activeInHierarchy)
        {
            SceneTransition.ChangeScene(StageManager.GetCurrent().GetNextStageName());
            return;
        }
    }
    //�X�e�[�W����E�o
    void Quit()
    {
        if (StageManager.GetCurrent().gameObject.activeInHierarchy)
        {
            SceneTransition.ChangeScene(StageManager.GetCurrent().GetQuitSceneName());
        }
    }

    void UpdateView()
    {
        float cameraTiltSpeed = cameraTiltSpeedSlider.GetScaledOutputValue();
        //??��?��J??��?��??��?��??��?��??��?��??��?��̋p??��?��l??��?��??��?��??��?��K??��?��??��?��͈͂Ɏ�??��?��߂�
        cameraAngle = Mathf.Clamp(
            cameraAngle, MinCameraAngle, MaxCameraAngle);
        //??��?��J??��?��??��?��??��?��??��?��??��?��̋p??��?��l??��?��??��?��??��?��C??��?��[??��?��W??��?��??��?��??��?��O
        easedCameraAngle = Mathf.Lerp(
            easedCameraAngle, cameraAngle, cameraTiltSpeed);
        //??��?��J??��?��??��?��??��?��??��?��??��?��̋�??��?��??��?��??��?��??��?��??��?��C??��?��[??��?��W??��?��??��?��??��?��O
        easedCameraDistance = Mathf.Lerp(
            easedCameraDistance, cameraDistance, cameraTiltSpeed);
        //??��?��O??��?��t??��?��??��?��??��?��[??��?��??��?��??��?��??��?��??��?��??��?��̉�]??��?��̍�??��?��ɉ�??��?��??��?��??��?��ăJ??��?��??��?��??��?��??��?��??��?��̌X??��?��??��?��??��?��p??��?��??��?��??��?��??��?��??��?��߂�
        cameraTiltRot =
            cameraTiltRot * (prevRot * Quaternion.Inverse(transform.rotation));

        //??��?��J??��?��??��?��??��?��??��?��??��?��̌X??��?��??��?��??��?��p??��?��̍�??��?��??��?��??��?��??��?��??��?��??��?��??��?��߂�
        float cameraTiltAmount = Mathf.Abs(
            Quaternion.Angle(cameraTiltRot, Quaternion.identity)) / 180;

        cameraTiltDiffuse = Mathf.Clamp(
            cameraTiltDiffuse + cameraTiltAmount
            * cameraFlipMotionMultiplySlider.GetScaledOutputValue(),
            0, cameraTiltAmount * cameraTiltSpeed);

        //??��?��J??��?��??��?��??��?��??��?��??��?��̌X??��?��??��?��??��?��p??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��
        cameraTiltRot = Quaternion.Slerp(
            cameraTiltRot, Quaternion.identity,
            cameraTiltDiffuse / KX_netUtil.RangeMap(cameraTiltAmount, 1, 0, 1, 0.001f));
        //??��?��܂�??��?��L??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��p??��?��x??��?��ɃJ??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��??��?��
        view.transform.localEulerAngles = new Vector3(easedCameraAngle, 0, 0);
        //??��?��J??��?��??��?��??��?��??��?��??��?��̌X??��?��??��?��??��?��p??��?��ɉ�??��?��??��?��??��?��ăJ??��?��??��?��??��?��??��?��??��?��??��?��??��?��X??��?��??��?��??��?��??��?��
        view.transform.rotation =
            cameraTiltRot * view.transform.rotation;
        //??��?��J??��?��??��?��??��?��??��?��??��?��̈ʒu??��?��??��?��??��?��J??��?��??��?��??��?��??��?��??��?��??��?��??��?��猩??��?��Č�??��?��??��?��ɂ�??��?��??��?��
        view.transform.localPosition =
            view.transform.localRotation * new Vector3(0, 0, -1)
            * easedCameraDistance;
        //??��?��J??��?��??��?��??��?��??��?��??��?��̋�??��?��??��?��??��?��??��?��??��?��f??��?��t??��?��H??��?��??��?��??��?��g??��?��l??��?��??��?��
        cameraDistance = defaultCameraDistance;
        prevRot = transform.rotation;
    }

    //����LiveEntity������ʉ���炷
    public void PlayAsSE(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
    }
    public void SetDirection(float setDirection)
    {
        if (isAllowCassetteUpdate)
        {
            direction = setDirection;
        }
    }
    public void SetCameraAngle(float setCameraAngle)
    {
        if (updating)
        {
            cameraAngle = setCameraAngle;
        }
    }

    public static LiveEntity Spawn(ResourcePalette palette,
        Vector3 setPosition, Quaternion setRotation, bool setUserControl,
        string setTeamID,
        string[] inventoryCharaID, int[] setTeamMember, int cassetteIndex)
    {
        LiveEntity liveEntity = Instantiate(palette.GetLiveEntity().gameObject,
            setPosition, setRotation).GetComponent<LiveEntity>();
        liveEntity.Init(setRotation, setUserControl, setTeamID,
            inventoryCharaID, setTeamMember, cassetteIndex);
        return liveEntity;
    }
}