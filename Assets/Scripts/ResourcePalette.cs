using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateResourcePalette")]
public class ResourcePalette : ScriptableObject
{
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
}