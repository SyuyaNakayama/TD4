using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//�f�o�b�O�p�F�I�����ڂ�������悤�ɂ��邽�߂����̉��X�N���v�g
public class test_select_color : MonoBehaviour
{
    //�F��ύX����I�u�W�F�N�g�Ƃ���ɃA�^�b�`����Ă�X�N���v�g
    public GameObject testObj;
    private test_select tSel;
    //�X�e�[�W�ԍ�
    private int stageNum = 0;

    void Start()
    {
        tSel = testObj.GetComponent<test_select>();
    }

    void Update()
    {
        //�I�����ڂɂ���ĐF���ς��悤�Ƀ}�e���A���̐F��ύX
        stageNum = tSel.GetSelectNum();
        switch (stageNum)
        {
            case 0:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 0);
                break;
            case 1:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(0, 255, 0, 0);
                break;
            case 2:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(0, 0, 255, 0);
                break;
            default:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 0);
                break;
        }
    }
}
