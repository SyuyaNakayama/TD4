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
        //接続されているコントローラの数を調べる
        int controllerNum = 0;
        /*string[] controllers = Input.GetJoystickNames();
        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] != "")
            {
                controllerNum++;
            }
        }*/

        //一台も接続されてなければキーボード専用のテキストに切り替える
        if (controllerNum <= 0)
        {
            button.SetText(keyBoardControlText);
            buttonAttack.SetText(keyBoardAttackButtonText);
        }
        else
        {
            //接続されていればコントローラ専用テキストに切り替える
            button.SetText(gamePadControlText);
            buttonAttack.SetText(gamePadAttackButtonText);
        }
    }
}
