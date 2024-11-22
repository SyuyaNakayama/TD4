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

    float life = 1;//残り体力の割合
    public float GetLife()
    {
        return life;
    }
    bool shield;//これがtrueの間は技による無敵時間
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
    int ghostTimeFrame;//ヒット後無敵時間
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

        //操作しているキャラクターのカメラとオーディオリスナーをオンにする
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

        //使っていないキャラクターオブジェクトを消す
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

        //キャラクターオブジェクトを生成
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

        //キャラクターの更新処理
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

        //トリガー入力用の変数を更新
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

    //OnCollisionStay??ｿｽ?ｿｽ??ｿｽ?ｿｽOnTriggerStay??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽZ??ｿｽ?ｿｽﾟにゑｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾖ撰ｿｽ
    void OnHit(Collider col)
    {
        if (col.gameObject.GetComponent<AttackArea>() != null)
        {
            AttackHit(col.gameObject.GetComponent<AttackArea>());
        }

        //??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾅ各??ｿｽ?ｿｽh??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽN??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽX??ｿｽ?ｿｽﾌ固有??ｿｽ?ｿｽﾚ触??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾄゑｿｽ
        LiveEntityOnHit(col);
    }

    //??ｿｽ?ｿｽe??ｿｽ?ｿｽh??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽN??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽX??ｿｽ?ｿｽﾌ固有??ｿｽ?ｿｽX??ｿｽ?ｿｽV??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽi??ｿｽ?ｿｽh??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽN??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽX??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾅオ??ｿｽ?ｿｽ[??ｿｽ?ｿｽo??ｿｽ?ｿｽ[??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽC??ｿｽ?ｿｽh??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾄ使??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽj
    protected virtual void LiveEntityUpdate()
    {
    }

    //TODO:??ｿｽ?ｿｽJ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽI??ｿｽ?ｿｽﾕで必??ｿｽ?ｿｽv??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾛゑｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽf??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽA??ｿｽ?ｿｽs??ｿｽ?ｿｽv??ｿｽ?ｿｽﾈゑｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ
    //??ｿｽ?ｿｽe??ｿｽ?ｿｽh??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽN??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽX??ｿｽ?ｿｽﾌ固有??ｿｽ?ｿｽﾕ突擾ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽi??ｿｽ?ｿｽh??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽN??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽX??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾅオ??ｿｽ?ｿｽ[??ｿｽ?ｿｽo??ｿｽ?ｿｽ[??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽC??ｿｽ?ｿｽh??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾄ使??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽj

    protected virtual void LiveEntityOnHit(Collider col)
    {
        Item item = col.GetComponent<Item>();
        if (item)
        {
            //?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｾ?謗･隗ｦ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽA?ｿｽ?ｽｿ?ｽｽC?ｿｽ?ｽｿ?ｽｽe?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽz?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｾ芽ｿｽ莨夲ｽｿ?ｽｽ
            //(?ｿｽ?ｽｿ?ｽｽs?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｾ峨い?ｿｽ?ｽｿ?ｽｽC?ｿｽ?ｽｿ?ｽｽe?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ隰ｫ?ｽｾ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ`?ｿｽ?ｽｿ?ｽｽ[?ｿｽ?ｽｿ?ｽｽg?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽh?ｿｽ?ｽｿ?ｽｽ~?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ鬩搾ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｾ溘ｑ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｾ後ｈ縺??ｿｽ?ｽｿ?ｽｽ?ｾ域蒔?ｿｽ?ｽｿ?ｽｽu?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｾ?繧托ｽｿ?ｽｽ?ｿｽ?ｽｿ?ｽｽ?ｾ懊ｑ?ｽｿ?ｽｽ)
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

    //??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾄゑｿｽﾅゑｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾔは地??ｿｽ?ｿｽ`??ｿｽ?ｿｽﾉ触??ｿｽ?ｿｽ??ｿｽ?ｿｽﾄゑｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾉ托ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾈゑｿｽ??ｿｽ?ｿｽﾈゑｿｽ
    public void DisAllowGroundSet()
    {
        if (isAllowCassetteUpdate)
        {
            allowGroundSet = false;
        }
    }

    //??ｿｽ?ｿｽU??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ?ｿｽ??ｿｽ?ｿｽﾛにゑｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾄゑｿｽ
    void AttackHit(AttackArea attackArea)
    {
        LiveEntity attacker = attackArea.GetAttacker();

        //??ｿｽ?ｿｽU??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽt??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾔ、??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾂ厄ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾈ外??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ攻??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾈゑｿｽ
        if (IsLive() && !IsShield() && ghostTimeFrame <= 0
            && (attacker == null
                || attacker.GetTeamID() != teamID))
        {
            //??ｿｽ?ｿｽM??ｿｽ?ｿｽ~??ｿｽ?ｿｽb??ｿｽ?ｿｽN??ｿｽ?ｿｽﾈゑｿｽf??ｿｽ?ｿｽ[??ｿｽ?ｿｽ^??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ撰ｿｽ??ｿｽ?ｿｽl??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌまま使??ｿｽ?ｿｽ??ｿｽ?ｿｽ
            float damageValue = attackArea.GetData().power;
            int ghostTime = attackArea.GetData().ghostTime;
            //??ｿｽ?ｿｽL??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ攻??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾈゑｿｽ_??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ[??ｿｽ?ｿｽW??ｿｽ?ｿｽl??ｿｽ?ｿｽﾆ厄ｿｽ??ｿｽ?ｿｽG??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾔゑｿｽ??ｿｽ?ｿｽZ??ｿｽ?ｿｽo
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
    //ダメージ処理
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
    //ヒットバック
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
    //生きているか
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
    //技による無敵状態か
    public bool IsShield()
    {
        return shieldable && shield;
    }
    public bool IsGhostTime()
    {
        return ghostTimeFrame > 0;
    }
    //ダメージを受ける状態か
    public bool IsDamageTakeable()
    {
        return !IsShield() && !IsGhostTime();
    }
    //操作できる状態か
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
    //死んでいたら復活
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
    //ゴールエリアに触れた時の処理
    void Clear()
    {
        goaled = true;
    }
    //次のステージとして設定されているシーンへ飛ぶ
    void NextStage()
    {
        if (StageManager.GetCurrent().gameObject.activeInHierarchy)
        {
            SceneTransition.ChangeScene(StageManager.GetCurrent().GetNextStageName());
            return;
        }
    }
    //ステージから脱出
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
        //??ｿｽ?ｿｽJ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ仰角??ｿｽ?ｿｽl??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽK??ｿｽ?ｿｽ??ｿｽ?ｿｽﾍ囲に趣ｿｽ??ｿｽ?ｿｽﾟゑｿｽ
        cameraAngle = Mathf.Clamp(
            cameraAngle, MinCameraAngle, MaxCameraAngle);
        //??ｿｽ?ｿｽJ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ仰角??ｿｽ?ｿｽl??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽC??ｿｽ?ｿｽ[??ｿｽ?ｿｽW??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽO
        easedCameraAngle = Mathf.Lerp(
            easedCameraAngle, cameraAngle, cameraTiltSpeed);
        //??ｿｽ?ｿｽJ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ具ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽC??ｿｽ?ｿｽ[??ｿｽ?ｿｽW??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽO
        easedCameraDistance = Mathf.Lerp(
            easedCameraDistance, cameraDistance, cameraTiltSpeed);
        //??ｿｽ?ｿｽO??ｿｽ?ｿｽt??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ[??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ会ｿｽ]??ｿｽ?ｿｽﾌ搾ｿｽ??ｿｽ?ｿｽﾉ会ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾄカ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ傾??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽp??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾟゑｿｽ
        cameraTiltRot =
            cameraTiltRot * (prevRot * Quaternion.Inverse(transform.rotation));

        //??ｿｽ?ｿｽJ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ傾??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽp??ｿｽ?ｿｽﾌ搾ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾟゑｿｽ
        float cameraTiltAmount = Mathf.Abs(
            Quaternion.Angle(cameraTiltRot, Quaternion.identity)) / 180;

        cameraTiltDiffuse = Mathf.Clamp(
            cameraTiltDiffuse + cameraTiltAmount
            * cameraFlipMotionMultiplySlider.GetScaledOutputValue(),
            0, cameraTiltAmount * cameraTiltSpeed);

        //??ｿｽ?ｿｽJ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ傾??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽp??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ
        cameraTiltRot = Quaternion.Slerp(
            cameraTiltRot, Quaternion.identity,
            cameraTiltDiffuse / KX_netUtil.RangeMap(cameraTiltAmount, 1, 0, 1, 0.001f));
        //??ｿｽ?ｿｽﾜゑｿｽ??ｿｽ?ｿｽL??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽp??ｿｽ?ｿｽx??ｿｽ?ｿｽﾉカ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ
        view.transform.localEulerAngles = new Vector3(easedCameraAngle, 0, 0);
        //??ｿｽ?ｿｽJ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ傾??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽp??ｿｽ?ｿｽﾉ会ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾄカ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽX??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ
        view.transform.rotation =
            cameraTiltRot * view.transform.rotation;
        //??ｿｽ?ｿｽJ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ位置??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽJ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ迪ｩ??ｿｽ?ｿｽﾄ鯉ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾉゑｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ
        view.transform.localPosition =
            view.transform.localRotation * new Vector3(0, 0, -1)
            * easedCameraDistance;
        //??ｿｽ?ｿｽJ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾌ具ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽf??ｿｽ?ｿｽt??ｿｽ?ｿｽH??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽg??ｿｽ?ｿｽl??ｿｽ?ｿｽ??ｿｽ?ｿｽ
        cameraDistance = defaultCameraDistance;
        prevRot = transform.rotation;
    }

    //このLiveEntityから効果音を鳴らす
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