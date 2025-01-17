using UnityEngine;
using UnityEngine.InputSystem;
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
    string gamepadControlText;
    [TextArea(1, 16), SerializeField]
    string gamepadAttackButtonText;

    void Update()
    {
        //接続されているコントローラの数を調べる
        int controllerNum =
            Gamepad.all.ToArray().Length
            + Joystick.all.ToArray().Length;

        //一台も接続されてなければキーボード専用のテキストに切り替える
        if (controllerNum <= 0)
        {
            button.SetText(keyBoardControlText);
            buttonAttack.SetText(keyBoardAttackButtonText);
        }
        else
        {
            //接続されていればコントローラ専用テキストに切り替える
            button.SetText(gamepadControlText);
            buttonAttack.SetText(gamepadAttackButtonText);
        }
    }
}