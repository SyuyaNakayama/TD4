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
        //接続されているコントローラの名前を調べる
        var controllers = Input.GetJoystickNames();

        //一台も接続されてなければキーボード専用のテキストに切り替える
        if (controllers[0] != "")
        {
            button.SetText("[W][A][S][D]：カメラ調整\r\n[↑][↓][←][→]：移動　[SPACE]：ジャンプ\r\n");
            buttonAttack.SetText("[Z]");
        }
        else
        {
            //接続されていればコントローラ専用テキストに切り替える
            button.SetText("右スティック：カメラ調整\r\n左スティック：移動　[A]：ジャンプ\r\n");
            buttonAttack.SetText("[B]");
        }
    }
}
