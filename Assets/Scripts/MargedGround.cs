using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����̘f���I�u�W�F�N�g����̘f���Ƃ݂Ȃ��@�\
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
        //�S�C���X�^���X������ϐ����X�V
        List<MargedGround> allInstancesList =
            new List<MargedGround>(allInstances);
        allInstancesList.RemoveAll(where => !where || where == this);
        allInstances = allInstancesList.ToArray();
        Array.Resize(ref allInstances, allInstances.Length + 1);
        allInstances[allInstances.Length - 1] = this;
    }
}
