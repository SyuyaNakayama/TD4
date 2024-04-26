using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_select : MonoBehaviour
{
    //�Q�[���}�l�[�W���[�̃I�u�W�F�N�g�ƃX�N���v�g
    public GameObject gmObj;
    private GameManager gm;
    //�f�t�H���g�V�[����
    private string sceneName = "test_stage0";
    //�X�e�[�W�ԍ��A�ŏ����ƍő吔
    private int stageNum = 0;
    private const int minStageNum = 0;
    private const int maxStageNum = 2;

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
            if(stageNum < minStageNum)
            {
                stageNum = maxStageNum;
            }
            ChangeTargetStage();
            Debug.Log("test_stage" +  stageNum);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            stageNum++;
            if (stageNum > maxStageNum)
            {
                stageNum = minStageNum;
            }
            ChangeTargetStage();
            Debug.Log("test_stage" + stageNum);
        }
        //�X�y�[�X�L�[�Ō���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gm.ChangeScene(sceneName);
        }
    }
    //�ړ���̃V�[�����̕ύX
    private void ChangeTargetStage()
    {
        sceneName = "test_stage" + stageNum;
    }
    //�I�𒆂̃X�e�[�W�ԍ��̎擾
    public int GetSelectNum()
    {
        Debug.Log(stageNum);
        return stageNum;
    }
}
