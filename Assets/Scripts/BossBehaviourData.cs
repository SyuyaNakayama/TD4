using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateBossBehaviourData")]
public class BossBehaviourData : ScriptableObject
{
    [SerializeField]
    string[] attackChart = { };
    public string[] GetAttackChart()
    {
        return attackChart;
    }
}