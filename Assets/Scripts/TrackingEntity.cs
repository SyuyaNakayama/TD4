using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackingEntity : MonoBehaviour
{
    //�ǔ�����G���e�B�f�B
    [SerializeField]
    GameObject target;
    //�^�[�Q�b�g�̉�]�p
    [SerializeField]
    GameObject rotate;
    //���g
    private GameObject me;
    //�ǂݍ��܂�����W�Q
    private Vector3[] loadPos = new Vector3[121];
    private Quaternion[] loadRot = new Quaternion[1211];
    //�f�[�^�Q�̍ő吔
    private const int dataNum = 120;
    void Start()
    {
        me = this.gameObject;
        for(int i = 0; i < dataNum; i++)
        {
            loadPos[i] = new Vector3 (0, 100, 0);
            loadRot[i] = new Quaternion(0, 0, 0, 0);
        }

    }
    void Update()
    {
        //�^�[�Q�b�g�̃f�[�^����
        loadPos[0] = target.transform.position;
        loadRot[0] = rotate.transform.rotation;
        Debug.Log(rotate.transform.rotation);
        //�f�[�^�����炷
        for(int i = dataNum;i > 0; i--)
        {
            loadPos[i] = loadPos[i-1];
            loadRot[i] = loadRot[i-1];
        }
        //�Ō����K�p������
        me.transform.position = new Vector3(loadPos[dataNum].x, loadPos[dataNum].y, loadPos[dataNum].z);
        me.transform.rotation = new Quaternion(loadRot[dataNum].x, loadRot[dataNum].y, loadRot[dataNum].z, loadRot[dataNum].w);
    }
}
