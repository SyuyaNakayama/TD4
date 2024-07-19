using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//複数の惑星オブジェクトを一つの惑星とみなす機能
public class MargedGround : MonoBehaviour
{
    [System.Serializable]
    public struct GroundCluster
    {
        public Collider[] colliders;
    }

    [SerializeField]
    GroundCluster[] groundClusters = { };
    public GroundCluster[] GetGroundClusters()
    {
        return groundClusters;
    }
}
