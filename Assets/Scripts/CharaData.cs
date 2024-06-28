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