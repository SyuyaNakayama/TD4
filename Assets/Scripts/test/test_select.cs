using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class test_select : MonoBehaviour
{
    //�Q�[���}�l�[�W���[�̃I�u�W�F�N�g�ƃX�N���v�g
    public GameObject gmObj;
    private GameManager gm;
    //�f�t�H���g�V�[����
    private string sceneName = "stage_0";
    //�X�e�[�W�ԍ��ƍő吔
    public int stageNum = 0;
    private const int maxStageNum = 3;

    private void Start()
    {
        gm = gmObj.GetComponent<GameManager>();
    }
    void Update()
    {
        //���E���L�[�őI��
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            stageNum--;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            stageNum++;
        }
        // 0~2�͈̔͂Ń��[�v������
        stageNum = (int)Mathf.Repeat(stageNum, maxStageNum);
        ChangeTargetStage();
        Debug.Log("stage_" + stageNum);
        //�X�y�[�X�L�[�Ō���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gm.ChangeScene(sceneName);
        }
    }
    //�ړ���̃V�[�����̕ύX
    private void ChangeTargetStage()
    {
        sceneName = "stage_" + stageNum;
    }
    //�I�𒆂̃X�e�[�W�ԍ��̎擾
    public int GetSelectNum()
    {
        Debug.Log(stageNum);
        return stageNum;
    }
}
