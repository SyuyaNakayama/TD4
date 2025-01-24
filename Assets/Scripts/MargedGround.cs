using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//複数の惑星オブジェクトを一つの惑星とみなす機能
public class MargedGround : MonoBehaviour
{
    [Serializable]
    public struct GroundCluster
    {
        public Collider[] colliders;
    }

    static MargedGround[] allInstances = { };
    public static MargedGround[] GetAllInstances()
    {
        return KX_netUtil.CopyArray<MargedGround>(allInstances);
    }

    [SerializeField]
    GroundCluster[] groundClusters = { };
    public GroundCluster[] GetGroundClusters()
    {
        return groundClusters;
    }

    void FixedUpdate()
    {
        //全インスタンスを入れる変数を更新
        List<MargedGround> allInstancesList =
            new List<MargedGround>(allInstances);
        allInstancesList.RemoveAll(where => !where || where == this);
        allInstances = allInstancesList.ToArray();
        Array.Resize(ref allInstances, allInstances.Length + 1);
        allInstances[allInstances.Length - 1] = this;
    }
}
