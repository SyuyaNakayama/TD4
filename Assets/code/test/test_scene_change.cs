using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_scene_change : MonoBehaviour
{
    //�Q�[���}�l�[�W���[�̃I�u�W�F�N�g�Ƃ��̃X�N���v�g
    public GameObject gmObj;
    private GameManager gm;
    //�ړ����������V�[����
    public string sceneName;

    private void Start()
    {
        gm = gmObj.GetComponent<GameManager>();
    }


    void Update()
    {
        //�X�y�[�X�L�[�ŃV�[���ړ�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gm.ChangeScene(sceneName);
        }
        //���f�o�b�O�p�FS�L�[�Ńe�X�g�p�}�b�v�Ɉړ�
        if (Input.GetKeyDown(KeyCode.S))
        {
            gm.ChangeScene("SampleScene");
        }
    }
}
