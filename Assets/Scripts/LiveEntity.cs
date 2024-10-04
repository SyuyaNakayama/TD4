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
    float hpAmount = 1;//?ｿｽc?ｿｽ?ｿｽﾌ力の奇ｿｽ?ｿｽ?ｿｽ
    public float GetHPAmount()
    {
        return hpAmount;
    }
    bool shield;//?ｿｽ?ｿｽ?ｿｽ黷ｪtrue?ｿｽﾌ間は技?ｿｽﾉゑｿｽ髢ｳ?ｿｽG?ｿｽ?ｿｽ?ｿｽ?ｿｽ
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
    int ghostTimeFrame;//?ｿｽq?ｿｽb?ｿｽg?ｿｽ纐ｳ?ｿｽG?ｿｽ?ｿｽ?ｿｽ?ｿｽ
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

        //全インスタンスを入れる変数を更新
        List<LiveEntity> allInstancesList =
            new List<LiveEntity>(allInstances);
        allInstancesList.RemoveAll(where => !where || where == this);
        allInstances = allInstancesList.ToArray();
        Array.Resize(ref allInstances, allInstances.Length + 1);
        allInstances[allInstances.Length - 1] = this;

        if (view != null)
        {
            //?ｿｽJ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ仰角?ｿｽl?ｿｽ?ｿｽ?ｿｽK?ｿｽ?ｿｽﾍ囲に趣ｿｽ?ｿｽﾟゑｿｽ
            cameraAngle = Mathf.Clamp(
                cameraAngle, minCameraAngle, maxCameraAngle);
            //?ｿｽJ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ仰角?ｿｽl?ｿｽ?ｿｽ?ｿｽC?ｿｽ[?ｿｽW?ｿｽ?ｿｽ?ｿｽO
            easedCameraAngle = Mathf.Lerp(
                easedCameraAngle, cameraAngle, maxCameraTiltDiffuse);
            //?ｿｽJ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ具ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽC?ｿｽ[?ｿｽW?ｿｽ?ｿｽ?ｿｽO
            easedCameraDistance = Mathf.Lerp(
                easedCameraDistance, cameraDistance, maxCameraTiltDiffuse);
            //?ｿｽO?ｿｽt?ｿｽ?ｿｽ?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ会ｿｽ]?ｿｽﾌ搾ｿｽ?ｿｽﾉ会ｿｽ?ｿｽ?ｿｽ?ｿｽﾄカ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ傾?ｿｽ?ｿｽ?ｿｽp?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾟゑｿｽ
            cameraTiltRot =
                cameraTiltRot * (prevRot * Quaternion.Inverse(transform.rotation));

            //?ｿｽJ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ傾?ｿｽ?ｿｽ?ｿｽp?ｿｽﾌ搾ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾟゑｿｽ
            float cameraTiltAmount = Mathf.Abs(
                Quaternion.Angle(cameraTiltRot, Quaternion.identity)) / 180;

            cameraTiltDiffuse = Mathf.Clamp(
                cameraTiltDiffuse + cameraTiltAmount * cameraFlipMotionMultiply,
                0, cameraTiltAmount * maxCameraTiltDiffuse);

            //?ｿｽJ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ傾?ｿｽ?ｿｽ?ｿｽp?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
            cameraTiltRot = Quaternion.Slerp(
                cameraTiltRot, Quaternion.identity,
                cameraTiltDiffuse / KX_netUtil.RangeMap(cameraTiltAmount, 1, 0, 1, 0.001f));
            //?ｿｽﾜゑｿｽ?ｿｽL?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?す?ｿｽp?ｿｽx?ｿｽﾉカ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
            view.transform.localEulerAngles = new Vector3(easedCameraAngle, 0, 0);
            //?ｿｽJ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ傾?ｿｽ?ｿｽ?ｿｽp?ｿｽﾉ会ｿｽ?ｿｽ?ｿｽ?ｿｽﾄカ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽX?ｿｽ?ｿｽ?ｿｽ?ｿｽ
            view.transform.rotation =
                cameraTiltRot * view.transform.rotation;
            //?ｿｽJ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ位置?ｿｽ?ｿｽ?ｿｽJ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ迪ｩ?ｿｽﾄ鯉ｿｽ?側?ｿｽﾉゑｿｽ?ｿｽ?ｿｽ
            view.transform.localPosition =
                view.transform.localRotation * new Vector3(0, 0, -1)
                * easedCameraDistance;
            //?ｿｽJ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ具ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽf?ｿｽt?ｿｽH?ｿｽ?ｿｽ?ｿｽg?ｿｽl?ｿｽ?ｿｽ
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

        //?ｿｽX?ｿｽP?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽﾝ抵ｿｽﾉ搾ｿｽ?ｿｽ墲ｹ?ｿｽ?ｿｽ
        float scale = data.GetScale();
        transform.localScale = new Vector3(scale, scale, scale);

        //allowGroundSet?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽZ?ｿｽb?ｿｽg
        allowGroundSet = true;
        //shield?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽZ?ｿｽb?ｿｽg
        shield = false;
        //?ｿｽ\?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽZ?ｿｽb?ｿｽg
        facialExpressionName = "";
        //?ｿｽA?ｿｽj?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾊ常時?ｿｽﾌゑｿｽ?ｿｽﾌにゑｿｽ?ｿｽ?ｿｽ
        animationName = "idol";

        isAllowMove = IsActable();

        animationProgress = Mathf.Repeat(animationProgress + animationSpeed, 1);
        animationSpeed = 0;

        //?ｿｽX?ｿｽN?ｿｽ?ｿｽ?ｿｽv?ｿｽ^?ｿｽu?ｿｽ?ｿｽ?ｿｽI?ｿｽu?ｿｽW?ｿｽF?ｿｽN?ｿｽg?ｿｽ?ｿｽ?ｿｽ?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ難ｿｽ?ｿｽe?ｿｽ?ｿｽﾇみ出?ｿｽ?ｿｽ
        UpdateAttackMotion();

        if (IsLive() && !GetGoaled())
        {
            cadaverLifeTimeFrame = maxCadaverLifeTimeFrame;
            goalAnimationTimeFrame = maxGoalAnimationTimeFrame;

            if (IsActable())
            {
                //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾅ各?ｿｽh?ｿｽ?ｿｽ?ｿｽN?ｿｽ?ｿｽ?ｿｽX?ｿｽﾌ固有?ｿｽX?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ
                LiveEntityUpdate();
            }
            else
            {
                animationName = "damage";
            }

            if (IsPlayer())
            {
                //?ｿｽG?ｿｽ?ｿｽ?ｿｽ^?ｿｽ[?ｿｽL?ｿｽ[?ｿｽA?ｿｽ?ｿｽ?ｿｽj?ｿｽ?ｿｽ?ｿｽ[?ｿｽ{?ｿｽ^?ｿｽ?ｿｽ?ｿｽﾅゑｿｽ?ｿｽﾂでゑｿｽ?ｿｽE?ｿｽo
                if (Input.GetKey(KeyCode.Return)
                    || Input.GetKey("joystick button 2"))//TODO:?ｿｽN?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌキ?ｿｽ[?ｿｽR?ｿｽ[?ｿｽh?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽj?ｿｽ?ｿｽ?ｿｽ[?ｿｽ{?ｿｽ^?ｿｽ?ｿｽ?ｿｽﾌコ?ｿｽ[?ｿｽh?ｿｽﾉ擾ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽ
                {
                    Quit();
                }
            }
        }
        else
        {
            //?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
            attackMotionData = null;

            //?ｿｽJ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽo?ｿｽp?ｿｽﾌ位置?ｿｽﾉ抵ｿｽ?ｿｽ?ｿｽ
            cameraAngle = goaledCameraAngle;
            cameraDistance = goaledCameraDistance;
            if (IsPlayer())
            {
                //?ｿｽ?ｿｽ?ｿｽﾊゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
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
                        //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ{?ｿｽ^?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ逡懶ｿｽ?ｿｽ
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
                    //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ{?ｿｽ^?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ邇滂ｿｽﾌス?ｿｽe?ｿｽ[?ｿｽW?ｿｽ?ｿｽ
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
            //?ｿｽﾌのパ?ｿｽ[?ｿｽc?ｿｽﾌト?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽX?ｿｽt?ｿｽH?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽf?ｿｽt?ｿｽH?ｿｽ?ｿｽ?ｿｽg?ｿｽ?ｿｽﾔゑｿｽ
            visual.transform.localScale = new Vector3(1, 1, 1);
            visual.transform.localPosition = Vector3.zero;
            //?ｿｽL?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ鯉ｿｽ?ｿｽ?ｿｽ?ｿｽﾚゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾖ鯉ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
            visualDirection += KX_netUtil.AngleDiff(visualDirection, direction)
                * directionTiltIntensity;
            visual.transform.localEulerAngles = new Vector3(0,
                visualDirection,
                0);
        }

        UpdateAnimation();

        if (visual != null)
        {
            //?ｿｽq?ｿｽb?ｿｽg?ｿｽ纐ｳ?ｿｽG?ｿｽ?ｿｽ?ｿｽﾔ抵ｿｽ?ｿｽﾈゑｿｽ_?ｿｽ?ｿｽ
            if ((ghostTimeFrame > 0 && Time.time % 0.1f < 0.05f)
                && IsLive() && !GetGoaled())
            {
                visual.transform.localScale = Vector3.zero;
            }
            //?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｯゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾈゑｿｽV?ｿｽF?ｿｽC?ｿｽN
            if (damageReactionTimeFrame > 0
                || (!IsLive() && cadaverLifeTimeFrame > shakeCadaverLifeTimeFrame))
            {
                visual.transform.localPosition +=
                    Vector3.Normalize(new Vector3(UnityEngine.Random.Range(1f, -1f),
                    UnityEngine.Random.Range(1f, -1f),
                    UnityEngine.Random.Range(1f, -1f))) * 0.2f; ;
            }
        }

        //prevAttackProgress?ｿｽ?ｿｽ?ｿｽX?ｿｽV
        prevAttackProgress = GetAttackProgress();
        //?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ進?ｿｽs?ｿｽx?ｿｽ?揄?ｿｽ
        attackProgress += 1 / Mathf.Max((float)attackTimeFrame, 1);
        attackProgress = Mathf.Clamp(attackProgress, 0, 1);

        ghostTimeFrame = Mathf.Max(0, ghostTimeFrame - 1);
        hitBackTimeFrame = Mathf.Max(0, hitBackTimeFrame - 1);
        damageReactionTimeFrame =
            Mathf.Max(0, damageReactionTimeFrame - 1);

        //?ｿｽS?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ逍ｳ?ｿｽG?ｿｽ?ｿｽ
        if (GetGoaled())
        {
            ghostTimeFrame = reviveGhostTimeFrame;
        }

        //?ｿｽ?ｿｽ?ｿｽG?ｿｽZ?ｿｽ?ｿｽ?ｿｽﾍバ?ｿｽb?ｿｽe?ｿｽ?ｿｽ?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
        //?ｿｽ?ｿｽ?ｿｽ?ｿｽﾘゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾆ回復ゑｿｽ?ｿｽﾘゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾉ厄ｿｽ?ｿｽG?ｿｽﾉなゑｿｽ驍ｩ?ｿｽﾛゑｿｽ?ｿｽﾌフ?ｿｽ?ｿｽ?ｿｽO?ｿｽ?ｿｽﾘゑｿｽﾖゑｿｽ?ｿｽ?ｿｽ
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

        //?ｿｽ?ｿｽ?ｿｽﾎらく?ｿｽ_?ｿｽ?ｿｽ?ｿｽ[?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｯてゑｿｽ?ｿｽﾈゑｿｽ?ｿｽ?ｿｽﾎ会ｿｽ
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

    //OnCollisionStay?ｿｽ?ｿｽOnTriggerStay?ｿｽ?ｿｽ?ｿｽ?ｿｽZ?ｿｽﾟにゑｿｽ?ｿｽ?ｿｽ?ｿｽﾖ撰ｿｽ
    void OnHit(Collider col)
    {
        if (col.gameObject.GetComponent<AttackArea>() != null)
        {
            AttackHit(col.gameObject.GetComponent<AttackArea>());
        }

        //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾅ各?ｿｽh?ｿｽ?ｿｽ?ｿｽN?ｿｽ?ｿｽ?ｿｽX?ｿｽﾌ固有?ｿｽﾚ触?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ
        LiveEntityOnHit(col);
    }

    //?ｿｽe?ｿｽh?ｿｽ?ｿｽ?ｿｽN?ｿｽ?ｿｽ?ｿｽX?ｿｽﾌ固有?ｿｽX?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽi?ｿｽh?ｿｽ?ｿｽ?ｿｽN?ｿｽ?ｿｽ?ｿｽX?ｿｽ?ｿｽ?ｿｽﾅオ?ｿｽ[?ｿｽo?ｿｽ[?ｿｽ?ｿｽ?ｿｽC?ｿｽh?ｿｽ?ｿｽ?ｿｽﾄ使?ｿｽ?ｿｽ?ｿｽj
    protected virtual void LiveEntityUpdate()
    {
    }

    //TODO:?ｿｽJ?ｿｽ?ｿｽ?ｿｽI?ｿｽﾕで必?ｿｽv?ｿｽ?ｿｽ?ｿｽﾛゑｿｽ?ｿｽ?ｿｽ?ｿｽf?ｿｽ?ｿｽ?ｿｽA?ｿｽs?ｿｽv?ｿｽﾈゑｿｽ?ｿｽ?ｿｽ?ｿｽ
    //?ｿｽe?ｿｽh?ｿｽ?ｿｽ?ｿｽN?ｿｽ?ｿｽ?ｿｽX?ｿｽﾌ固有?ｿｽﾕ突擾ｿｽ?ｿｽ?ｿｽ?ｿｽi?ｿｽh?ｿｽ?ｿｽ?ｿｽN?ｿｽ?ｿｽ?ｿｽX?ｿｽ?ｿｽ?ｿｽﾅオ?ｿｽ[?ｿｽo?ｿｽ[?ｿｽ?ｿｽ?ｿｽC?ｿｽh?ｿｽ?ｿｽ?ｿｽﾄ使?ｿｽ?ｿｽ?ｿｽj
    protected virtual void LiveEntityOnHit(Collider col)
    {

    }

    //?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾉ移行
    protected void SetAttackMotion(string name)
    {
        SetAttackMotion(data.SearchAttackMotion(name));
    }

    //?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾉ移行
    protected void SetAttackMotion(AttackMotionData attackMotion)
    {
        attackMotionData = attackMotion;
        attackTimeFrame = Mathf.Max(attackMotionData.GetData().totalFrame, 1);
        attackProgress = 0;
    }

    //?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
    protected bool IsAttacking()
    {
        return attackMotionData != null
            && (attackTimeFrame < 1 || prevAttackProgress < 1);
    }
    //?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾂ指?ｿｽ?ｿｽﾌ攻?ｿｽ?ｿｽ?ｿｽA?ｿｽN?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽs?ｿｽﾈゑｿｽ?ｿｽﾄゑｿｽ?ｿｽ驍ｩ
    protected bool IsAttacking(string name)
    {
        return IsAttacking() && attackMotionData.name == name;
    }
    //attackProgress?ｿｽ?ｿｽ?ｿｽw?ｿｽ?ｿｽﾌキ?ｿｽ[?ｿｽ|?ｿｽC?ｿｽ?ｿｽ?ｿｽg?ｿｽ?ｿｽﾊ過ゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
    protected bool IsHitKeyPoint(float keyPoint)
    {
        return KX_netUtil.IsIntoRange(
            keyPoint, prevAttackProgress, GetAttackProgress(),
            false, true);
    }
    //attackProgress?ｿｽ?ｿｽ?ｿｽw?ｿｽ?ｿｽﾌ範囲難ｿｽ?ｿｽA?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾍゑｿｽ?ｿｽﾌ範囲ゑｿｽ1?ｿｽt?ｿｽ?ｿｽ?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾅ通過ゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
    protected bool IsHitKeyPoint(Vector2 keyPoint)
    {
        return KX_netUtil.IsCrossingRange(
            prevAttackProgress, GetAttackProgress(),
            keyPoint.x, keyPoint.y,
            false, false);
    }

    //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽﾅゑｿｽ?ｿｽ?ｿｽﾔは地?ｿｽ`?ｿｽﾉ触?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾉ托ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾈゑｿｽ?ｿｽﾈゑｿｽ
    protected void DisAllowGroundSet()
    {
        allowGroundSet = false;
    }

    //?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｯゑｿｽ?ｿｽﾛにゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ
    void AttackHit(AttackArea attackArea)
    {
        LiveEntity attacker = attackArea.GetAttacker();

        //?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｯ付?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾔ、?ｿｽ?ｿｽ?ｿｽﾂ厄ｿｽ?ｿｽ?ｿｽ?ｿｽﾈ外?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ攻?ｿｽ?ｿｽ?ｿｽﾈゑｿｽ
        if (IsLive() && !IsShield() && ghostTimeFrame <= 0
            && (attacker == null
                || attacker.GetTeamID() != teamID))
        {
            //?ｿｽM?ｿｽ~?ｿｽb?ｿｽN?ｿｽﾈゑｿｽf?ｿｽ[?ｿｽ^?ｿｽ?ｿｽﾌ撰ｿｽ?ｿｽl?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌまま使?ｿｽ?ｿｽ
            float damageValue = attackArea.GetData().power;
            int ghostTime = attackArea.GetData().ghostTime;
            //?ｿｽL?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ攻?ｿｽ?ｿｽ?ｿｽﾈゑｿｽ_?ｿｽ?ｿｽ?ｿｽ[?ｿｽW?ｿｽl?ｿｽﾆ厄ｿｽ?ｿｽG?ｿｽ?ｿｽ?ｿｽﾔゑｿｽ?ｿｽZ?ｿｽo
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
    //?ｿｽﾌ力ゑｿｽ?ｿｽ?ｿｽ?ｿｽ轤ｵ?ｿｽA?ｿｽ?ｿｽ?ｿｽG?ｿｽ?ｿｽ?ｿｽﾔゑｿｽt?ｿｽ^
    void Damage(float damage, int setGhostTimeFrame)
    {
        hpAmount -= Mathf.Max(0, damage / data.GetLife());
        ghostTimeFrame = setGhostTimeFrame;
        repairCoolTimeFrame = maxRepairCoolTimeFrame;
        damageReactionTimeFrame = maxDamageReactionTimeFrame;
        //?ｿｽ_?ｿｽ?ｿｽ?ｿｽ[?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽﾂらす
        if (IsLive())
        {
            PlayAsSE(resourcePalette.GetDamageSE());
        }
        else
        {
            PlayAsSE(resourcePalette.GetDefeatSE());
            if (!IsPlayer() && GetData().GetWeaponedAttackMotionName() != "")
            {
                //?ｿｽA?ｿｽC?ｿｽe?ｿｽ?ｿｽ?ｿｽ?ｶ撰ｿｽ
                CharaChip current =
                    Instantiate(resourcePalette.GetCharaChip().gameObject,
                    transform.position, transform.rotation, transform)
                    .GetComponent<CharaChip>();
                current.SetData(GetData());
                current.gameObject.transform.parent = transform.parent;
            }
        }
    }
    //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾎゑｿｽ?ｿｽ?ｿｽ?ｿｽ
    void HitBack(Vector3 hitBackVec, int setHitBackTimeFrame)
    {
        movement = Quaternion.Inverse(transform.rotation)
            * hitBackVec;
        hitBackTimeFrame = setHitBackTimeFrame;
        attackMotionData = null;
    }

    //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ驍ｩ
    public bool IsLive()
    {
        return hpAmount > 0;
    }
    //?ｿｽZ?ｿｽﾉゑｿｽ髢ｳ?ｿｽG?ｿｽ?ｿｽﾔゑｿｽ
    public bool IsShield()
    {
        return shieldable && shield;
    }
    //?ｿｽ_?ｿｽ?ｿｽ?ｿｽ[?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｯ付?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾔゑｿｽ
    public bool IsDamageTakeable()
    {
        return !IsShield() && ghostTimeFrame <= 0;
    }
    //?ｿｽs?ｿｽ?ｿｽ?ｿｽﾅゑｿｽ?ｿｽ?ｿｽ?ｿｽﾔゑｿｽ
    public bool IsActable()
    {
        return hitBackTimeFrame <= 0;
    }
    //?ｿｽ?ｿｽ?ｿｽ?ｿｽﾍプ?ｿｽ?ｿｽ?ｿｽC?ｿｽ?ｿｽ?ｿｽ[?ｿｽ?ｿｽ
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

    //?ｿｽ?ｿｽ?ｿｽ?ｿｽﾅゑｿｽ?ｿｽ?ｿｽﾆゑｿｽ?ｿｽﾉゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄぶと包ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
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
    //?ｿｽS?ｿｽ[?ｿｽ?ｿｽ?ｿｽﾉ難ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ擾ｿｽ?ｿｽ?ｿｽ
    void Clear()
    {
        goaled = true;
    }
    //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽX?ｿｽe?ｿｽ[?ｿｽW?ｿｽﾌ趣ｿｽ?ｿｽﾉ設定さ?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽX?ｿｽe?ｿｽ[?ｿｽW?ｿｽﾖ進?ｿｽ?ｿｽ
    void NextStage()
    {
        if (StageManager.GetCurrent().gameObject.activeInHierarchy)
        {
            SceneTransition.ChangeScene(StageManager.GetCurrent().GetNextStageName());
            return;
        }
    }
    //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽX?ｿｽe?ｿｽ[?ｿｽW?ｿｽﾌ派?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾆゑｿｽ?ｿｽﾄ設定さ?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽV?ｿｽ[?ｿｽ?ｿｽ?ｿｽﾉ戻ゑｿｽ
    void Quit()
    {
        if (StageManager.GetCurrent().gameObject.activeInHierarchy)
        {
            SceneTransition.ChangeScene(StageManager.GetCurrent().GetQuitSceneName());
        }
    }

    //?ｿｽﾝ定さ?ｿｽ黷ｽ?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽf?ｿｽ[?ｿｽ^?ｿｽ?ｿｽﾇみ出?ｿｽ?ｿｽ?ｿｽﾄ趣ｿｽ?ｿｽs?ｿｽi?ｿｽ?ｿｽ?ｿｽs?ｿｽ?ｿｽ?ｿｽﾍ擾ｿｽﾉ呼ぶ）
    void UpdateAttackMotion()
    {
        //3D?ｿｽJ?ｿｽ[?ｿｽ\?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽZ?ｿｽb?ｿｽg
        Array.Resize(ref cursors, 0);

        //?ｿｽﾅ有?ｿｽ?ｿｽ?ｿｽ?ｿｽ|?ｿｽC?ｿｽ?ｿｽ?ｿｽg?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽZ?ｿｽb?ｿｽg
        Array.Resize(ref uniqueActDatas, 0);

        if (IsAttacking())
        {
            //3D?ｿｽJ?ｿｽ[?ｿｽ\?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ謫ｾ
            if (attackMotionData.GetCursors() != null)
            {
                cursors = attackMotionData.GetCursors();
            }

            //?ｿｽﾟ接攻?ｿｽ?ｿｽ
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

            //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽU?ｿｽ?ｿｽ
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

            //?ｿｽﾚ難ｿｽ
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

            //?ｿｽﾅ有?ｿｽ?ｿｽ?ｿｽ?ｿｽ|?ｿｽC?ｿｽ?ｿｽ?ｿｽg
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

            //?ｿｽ?ｿｽ?ｿｽG?ｿｽ?ｿｽ?ｿｽ?ｿｽ
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

            //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽs?ｿｽﾂ趣ｿｽ?ｿｽ?ｿｽ
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

            //?ｿｽ?ｿｽ?ｿｽﾊ会ｿｽ
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

            //?ｿｽA?ｿｽj?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ
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

        //?ｿｽU?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽo?ｿｽ?ｿｽ
        //?ｿｽﾜゑｿｽ?ｿｽﾍ近接攻?ｿｽ?ｿｽ?ｿｽﾌデ?ｿｽ[?ｿｽ^?ｿｽﾆ難ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ茨ｿｽ?ｿｽp?ｿｽ?ｿｽ
        Array.Resize(ref attackAreas, meleeAttackDatas.Length);
        //?ｿｽﾌ茨ｿｽ?ｿｽ?ｿｽﾌ攻?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾉ近接攻?ｿｽ?ｿｽ?ｿｽﾌデ?ｿｽ[?ｿｽ^?ｿｽ?ｿｽ?ｿｽ?ｿｽ
        for (int i = 0; i < attackAreas.Length; i++)
        {
            MeleeAttackAndCursorName currentData = meleeAttackDatas[i];

            //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾎ撰ｿｽ?ｿｽ?ｿｽ
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
        //?ｿｽs?ｿｽv?ｿｽﾈ攻?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
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

        //?ｿｽe?ｿｽ?ｿｽ?ｿｽo?ｿｽ?ｿｽ
        for (int i = 0; i < shotDatas.Length; i++)
        {
            ShotAndCursorName currentData = shotDatas[i];

            if (currentData.postMove)
            {
                //postMove?ｿｽ?ｿｽtrue?ｿｽﾈゑｿｽ1?ｿｽt?ｿｽ?ｿｽ?ｿｽ[?ｿｽ?ｿｽ?ｿｽx?ｿｽ轤ｹ?ｿｽ?ｿｽ
                shotDatas[i].postMove = false;
            }
            else
            {
                if (attackMotionData)
                {
                    AttackMotionData.Cursor currentCursor =
                        cursors[attackMotionData.SearchCursorIndex(currentData.cursorName)];

                    //?ｿｽ?ｿｽ?ｿｽ?ｿｽ
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

        //?ｿｽﾟ接、?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽU?ｿｽ?ｿｽ?ｿｽﾌデ?ｿｽ[?ｿｽ^?ｿｽ?ｿｽ?ｿｽ?ｿｽg?ｿｽp?ｿｽﾏみの要?ｿｽf?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
        Array.Resize(ref meleeAttackDatas, 0);

        List<ShotAndCursorName> shotDataList =
            new List<ShotAndCursorName>(shotDatas);
        shotDataList.RemoveAll(where => where.used);
        shotDatas = shotDataList.ToArray();
    }

    //?ｿｽﾝ定さ?ｿｽ黷ｽ?ｿｽA?ｿｽj?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽf?ｿｽ[?ｿｽ^?ｿｽ?ｿｽﾇみ出?ｿｽ?ｿｽ?ｿｽﾄ趣ｿｽ?ｿｽs?ｿｽi?ｿｽ?ｿｽ?ｿｽs?ｿｽ?ｿｽ?ｿｽﾍ擾ｿｽﾉ呼ぶ）
    void UpdateAnimation()
    {
        //?ｿｽﾜゑｿｽ?ｿｽp?ｿｽ[?ｿｽc?ｿｽﾌト?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽX?ｿｽt?ｿｽH?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽf?ｿｽt?ｿｽH?ｿｽ?ｿｽ?ｿｽg?ｿｽl?ｿｽﾉ托ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
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
        //?ｿｽ?ｿｽ?ｿｽﾝの擾ｿｽﾔにゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽA?ｿｽj?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ謫ｾ
        CharaData.Animation animationData =
            data.SearchAnimation(animationName);
        //?ｿｽg?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽX?ｿｽt?ｿｽH?ｿｽ[?ｿｽ?ｿｽ?ｿｽA?ｿｽj?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽK?ｿｽp
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

        //?ｿｽX?ｿｽL?ｿｽ?ｿｽ?ｿｽA?ｿｽj?ｿｽ?ｿｽ?ｿｽ[?ｿｽV?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽK?ｿｽp
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

        //?ｿｽ\?ｿｽ?ｿｽ?ｿｽK?ｿｽp
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

        //?ｿｽ?ｿｽ?ｿｽﾊ会ｿｽ
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

        //?ｿｽf?ｿｽt?ｿｽH?ｿｽ?ｿｽ?ｿｽg?ｿｽﾌテ?ｿｽN?ｿｽX?ｿｽ`?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽf?ｿｽ?ｿｽ?ｿｽﾉ貼?ｿｽ?ｿｽ
        for (int i = 0; i < meshes.Length; i++)
        {
            TextureSendData current = meshes[i];
            current.meshRenderer.materials[current.index].
                SetTexture(current.propertyName, data.GetDefaultTexture(i));
        }
        //?ｿｽf?ｿｽt?ｿｽH?ｿｽ?ｿｽ?ｿｽg?ｿｽﾌス?ｿｽv?ｿｽ?ｿｽ?ｿｽC?ｿｽg?ｿｽ?ｿｽ?ｿｽX?ｿｽv?ｿｽ?ｿｽ?ｿｽC?ｿｽg?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ_?ｿｽ?ｿｽ?ｿｽ[?ｿｽﾉ貼?ｿｽ?ｿｽ
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

        //?ｿｽ?ｿｽ?ｿｽﾝの擾ｿｽﾔにゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ\?ｿｽ?ｿｽ?ｿｽ?ｿｽ謫ｾ
        CharaData.FacialExpression facialData =
            data.SearchFacialExpression(facialExpressionName);
        //?ｿｽ\?ｿｽ?ｿｽﾌテ?ｿｽN?ｿｽX?ｿｽ`?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽf?ｿｽ?ｿｽ?ｿｽﾉ貼?ｿｽ?ｿｽ
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
        //?ｿｽ\?ｿｽ?ｿｽﾌス?ｿｽv?ｿｽ?ｿｽ?ｿｽC?ｿｽg?ｿｽ?ｿｽ?ｿｽX?ｿｽv?ｿｽ?ｿｽ?ｿｽC?ｿｽg?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ_?ｿｽ?ｿｽ?ｿｽ[?ｿｽﾉ貼?ｿｽ?ｿｽ
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

    //?ｿｽﾟ接ゑｿｽ?ｿｽ?ｿｽﾑ範囲攻?ｿｽ?ｿｽ
    void MeleeAttack(AttackMotionData.MeleeAttackData attackData, string cursorName)
    {
        Array.Resize(ref meleeAttackDatas, meleeAttackDatas.Length + 1);
        meleeAttackDatas[meleeAttackDatas.Length - 1].data = attackData;
        meleeAttackDatas[meleeAttackDatas.Length - 1].cursorName = cursorName;
    }
    //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽU?ｿｽ?ｿｽ
    void Shot(AttackMotionData.ShotData shotData, string cursorName, bool postMove)
    {
        Array.Resize(ref shotDatas, shotDatas.Length + 1);
        shotDatas[shotDatas.Length - 1].data = shotData;
        shotDatas[shotDatas.Length - 1].cursorName = cursorName;
        shotDatas[shotDatas.Length - 1].postMove = postMove;
        shotDatas[shotDatas.Length - 1].used = false;
    }
    //?ｿｽ?ｿｽ?ｿｽ?ｿｽLiveEntity?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾊ会ｿｽ?ｿｽ?ｿｽﾂらす
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