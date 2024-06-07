using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OperationInstructions : MonoBehaviour
{
    public TextMeshProUGUI button;
    public TextMeshProUGUI buttonAttack;

    void Update()
    {
        //�ڑ�����Ă���R���g���[���̖��O�𒲂ׂ�
        var controllers = Input.GetJoystickNames();

        //�����ڑ�����ĂȂ���΃L�[�{�[�h��p�̃e�L�X�g�ɐ؂�ւ���
        if (controllers[0] != "")
        {
            button.SetText("[W][A][S][D]�F�J��������\r\n[��][��][��][��]�F�ړ��@[SPACE]�F�W�����v\r\n");
            buttonAttack.SetText("[Z]");
        }
        else
        {
            //�ڑ�����Ă���΃R���g���[����p�e�L�X�g�ɐ؂�ւ���
            button.SetText("�E�X�e�B�b�N�F�J��������\r\n���X�e�B�b�N�F�ړ��@[A]�F�W�����v\r\n");
            buttonAttack.SetText("[B]");
        }
    }
}
