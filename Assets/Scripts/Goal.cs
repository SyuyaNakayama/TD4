using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    //�V�[���`�F���W�Ăяo�����̃Q�[���}�l�[�W���[�̃I�u�W�F�N�g�ƃX�N���v�g
    public GameObject gmObj;
    private GameManager gm;
    //�S�[��
    private GameObject goal;
    //�V�[���`�F���W�܂ł̗P�\����
    private int goalTimer = 300;
    //�擾�t���O
    private bool isGet = false;

    void Start()
    {
        gm = gmObj.GetComponent<GameManager>();
        goal = this.gameObject;
    }
    void Update()
    {
        if (isGet)
        {
            goalTimer--;
            if(goalTimer < 0)
            {
                gm.ChangeScene("stage_select");
            }
        }
    }
    //�����蔻��
    private void OnTriggerEnter(Collider other)
    {
        //�^�O��player�Ȃ�E��x���������������̂�
        if(other.gameObject.tag == "Player" && !isGet)
        {
            //�擾�t���O�I��
            isGet = true;
            //SetActive�Ŕ�\���ɂ���Ɠ����Ȃ��Ȃ�̂ŉ��ŁA�������@�������狳���Ă���
            goal.transform.position = new Vector3(10000, 10000, 10000);
        }
    }
}
