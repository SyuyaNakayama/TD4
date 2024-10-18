using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackingEntity : MonoBehaviour
{
    //�f�[�^�Q�̍ő吔
    const int dataNum = 20;

    //�ǔ�����G���e�B�f�B
    [SerializeField]
    GameObject target;
    //�\����
    [SerializeField]
    LineRenderer lineRenderer;

    //�ǂݍ��܂�����W�Q
    Vector3[] loadPos = new Vector3[dataNum];
    Quaternion[] loadRot = new Quaternion[dataNum];

    void Start()
    {
        for (int i = 0; i < dataNum; i++)
        {
            loadPos[i] = transform.position;
            loadRot[i] = transform.rotation;
        }
    }
    void FixedUpdate()
    {
        //�^�[�Q�b�g�̃f�[�^����
        loadPos[0] = target.transform.position;
        loadRot[0] = target.transform.rotation;
        //�f�[�^�����炷
        for (int i = dataNum - 1; i > 0; i--)
        {
            loadPos[i] = loadPos[i - 1];
            loadRot[i] = loadRot[i - 1];
        }
        //�Ō����K�p������
        transform.position = loadPos[dataNum - 1];
        transform.rotation = loadRot[dataNum - 1];
        //���_�f�[�^��lineRenderer�ɓn��
        lineRenderer.positionCount = loadPos.Length;
        lineRenderer.SetPositions(loadPos);
    }
}
