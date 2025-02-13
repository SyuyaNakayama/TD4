using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateResourcePalette")]
public class ResourcePalette : ScriptableObject
{
    [SerializeField]
    LiveEntity liveEntity;
    public LiveEntity GetLiveEntity()
    {
        return liveEntity;
    }
    [SerializeField]
    AttackArea attackArea;
    public AttackArea GetAttackArea()
    {
        return attackArea;
    }
    [SerializeField]
    Projectile projectile;
    public Projectile GetProjectile()
    {
        return projectile;
    }
    [SerializeField]
    CharaChip charaChip;
    public CharaChip GetCharaChip()
    {
        return charaChip;
    }
    [SerializeField]
    AudioClip damageSE;
    public AudioClip GetDamageSE()
    {
        return damageSE;
    }
    [SerializeField]
    AudioClip defeatSE;
    public AudioClip GetDefeatSE()
    {
        return defeatSE;
    }
    [SerializeField]
    GameObject damageEffect;
    public GameObject GetDamageEffect()
    {
        return damageEffect;
    }
    [SerializeField]
    GameObject guardEffect;
    public GameObject GetGuardEffect()
    {
        return guardEffect;
    }
    [SerializeField]
    GameObject despawnEffect;
    public GameObject GetDespawnEffect()
    {
        return despawnEffect;
    }
}