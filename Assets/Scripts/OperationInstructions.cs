using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OperationInstructions : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI button;
    [SerializeField]
    TextMeshProUGUI buttonAttack;
    [TextArea(1, 16), SerializeField]
    string keyBoardControlText;
    [TextArea(1, 16), SerializeField]
    string keyBoardAttackButtonText;
    [TextArea(1, 16), SerializeField]
    string gamePadControlText;
    [TextArea(1, 16), SerializeField]
    string gamePadAttackButtonText;

    void Update()
    {
        //�ڑ�����Ă���R���g���[���̐��𒲂ׂ�
        int controllerNum = 0;
        /*string[] controllers = Input.GetJoystickNames();
        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] != "")
            {
                controllerNum++;
            }
        }*/

        //�����ڑ�����ĂȂ���΃L�[�{�[�h��p�̃e�L�X�g�ɐ؂�ւ���
        if (controllerNum <= 0)
        {
            button.SetText(keyBoardControlText);
            buttonAttack.SetText(keyBoardAttackButtonText);
        }
        else
        {
            //�ڑ�����Ă���΃R���g���[����p�e�L�X�g�ɐ؂�ւ���
            button.SetText(gamePadControlText);
            buttonAttack.SetText(gamePadAttackButtonText);
        }
    }
}
