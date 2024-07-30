using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateCharaData")]
public class CharaData : ScriptableObject
{
    [System.Serializable]
    public struct IndexAndTexture
    {
        public int index;
        public Texture texture;
    }
    [System.Serializable]
    public struct IndexAndSprite
    {
        public int index;
        public Sprite sprite;
    }
    [System.Serializable]
    public struct FacialExpression
    {
        public string name;
        public IndexAndTexture[] indexAndTextures;
        public IndexAndSprite[] indexAndSprites;
    }
    [System.Serializable]
    public struct TransformAnimationKey
    {
        public Vector2 keyFrame;
        public int bodyPartIndex;
        public KX_netUtil.TransformData startTransform;
        public KX_netUtil.TransformData endTransform;
        public KX_netUtil.TransformSwitch ignoreTransform;
        public bool lerpAsEuler;
        public KX_netUtil.EaseType easeType;
        public float easePow;
    }
    [System.Serializable]
    public struct RigAnimationKey
    {
        public Vector2 keyFrame;
        public int animatorIndex;
        public int rigAnimationID;
    }
    [System.Serializable]
    public struct FacialExpressionKey
    {
        public Vector2 keyFrame;
        public string facialExpressionName;
    }
    [System.Serializable]
    public struct Animation
    {
        public string name;
        public int totalFrame;
        public TransformAnimationKey[] transformAnimationKeys;
        public RigAnimationKey[] rigAnimationKeys;
        public FacialExpressionKey[] facialExpressionKeys;
        public AttackMotionData.SEKey[] seKeys;
    }

    public const int totalStatusValue = 3000;
    public const float minLifeRatio = 0.2f;
    public const float maxLifeRatio = 0.8f;
    const float attackPowerRate = 0.4f;

    [SerializeField]
    string charaName;
    public string GetCharaName()
    {
        return charaName;
    }

    [SerializeField]
    Sprite iconGraph;
    public Sprite GetIconGraph()
    {
        return iconGraph;
    }

    [SerializeField]
    Color themeColor;
    public Color GetThemeColor()
    {
        return themeColor;
    }

    [SerializeField]
    [Range(minLifeRatio, maxLifeRatio)]
    float lifeRatio = 0.5f;
    public float GetLife()
    {
        return Mathf.Clamp(lifeRatio, minLifeRatio, maxLifeRatio)
        * totalStatusValue;
    }
    public float GetAttackPower()
    {
        return (1 - Mathf.Clamp(lifeRatio, minLifeRatio, maxLifeRatio))
        * totalStatusValue * attackPowerRate;
    }

    [SerializeField]
    float gravityScale = 0.5f;
    public float GetGravityScale()
    {
        return gravityScale;
    }

    [SerializeField]
    KX_netUtil.AxisSwitch dragAxis;
    public KX_netUtil.AxisSwitch GetDragAxis()
    {
        return dragAxis;
    }

    [SerializeField]
    float scale = 1;
    public float GetScale()
    {
        return scale;
    }

    [SerializeField]
    string defaultAttackMotionName;
    public string GetDefaultAttackMotionName()
    {
        return defaultAttackMotionName;
    }

    [SerializeField]
    string weaponedAttackMotionName;
    public string GetWeaponedAttackMotionName()
    {
        return weaponedAttackMotionName;
    }

    [SerializeField]
    AttackMotionData[] attackMotions = { };
    public AttackMotionData[] GetAttackMotions()
    {
        return attackMotions;
    }
    public AttackMotionData SearchAttackMotion(string name)
    {
        for (int i = 0; i < attackMotions.Length; i++)
        {
            if (name == attackMotions[i].name)
            {
                return attackMotions[i];
            }
        }
        return new AttackMotionData();
    }

    [SerializeField]
    KX_netUtil.TransformData[] defaultBodyPartsTransform = { };
    public KX_netUtil.TransformData GetDefaultBodyPartsTransform(int index)
    {
        return defaultBodyPartsTransform[index];
    }

    [SerializeField]
    Texture[] defaultTextures = { };
    public Texture GetDefaultTexture(int index)
    {
        return defaultTextures[index];
    }
    [SerializeField]
    Sprite[] defaultSprites = { };
    public Sprite GetDefaultSprite(int index)
    {
        return defaultSprites[index];
    }
    [SerializeField]
    Animation[] animations = { };
    public Animation SearchAnimation(string name)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].name == name)
            {
                return animations[i];
            }
        }

        return new Animation();
    }

    [SerializeField]
    FacialExpression[] facialExpressions = { };
    public FacialExpression SearchFacialExpression(string name)
    {
        for (int i = 0; i < facialExpressions.Length; i++)
        {
            if (facialExpressions[i].name == name)
            {
                return facialExpressions[i];
            }
        }

        return new FacialExpression();
    }
}