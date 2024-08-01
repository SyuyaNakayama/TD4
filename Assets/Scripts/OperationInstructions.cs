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
        //接続されているコントローラの数を調べる
        int controllerNum = 0;
        string[] controllers = Input.GetJoystickNames();
        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] != "")
            {
                controllerNum++;
            }
        }

        //一台も接続されてなければキーボード専用のテキストに切り替える
        if (controllerNum <= 0)
        {
            button.SetText("[W][A][S][D]：カメラ調整　[Z]：攻撃\r\n[↑][↓][←][→]：移動　[SPACE]：ジャンプ\r\n");
            buttonAttack.SetText("[Z]");
        }
        else
        {
            //接続されていればコントローラ専用テキストに切り替える
            button.SetText("右スティック：カメラ調整　[B]：攻撃\r\n左スティック：移動　[A]：ジャンプ\r\n");
            buttonAttack.SetText("[B]");
        }
    }
}
