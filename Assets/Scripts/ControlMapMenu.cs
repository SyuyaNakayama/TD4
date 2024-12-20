using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMapMenu : ControlMap
{
    [SerializeField]
    SpriteRenderer menuCursor;
    [SerializeField]
    SpriteRenderer menuCursor2;
    [SerializeField]
    Menu[] menus = { };

    bool isMouseInput;
    Vector3 keyInputVec;
    Vector3 prevKeyInputVec;
    bool input;
    public bool GetInput()
    {
        return input;
    }
    bool prevInput;
    public bool GetPrevInput()
    {
        return prevInput;
    }
    bool backInput;
    public bool GetBackInput()
    {
        return backInput;
    }
    int selectIndex;
    public int GetSelectIndex()
    {
        return selectIndex;
    }
    Menu currentMenu;
    Menu prevCurrentMenu;
    int menuPieceIndex;
    bool menuPieceControlling;
    bool prevHoldInput;
    Vector3 cursorPos;
    Vector3 cursorPos2;

    protected override void ControlMapUpdate()
    {
        //何かメニューを開いているか
        currentMenu = null;
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].IsCurrentMenu())
            {
                currentMenu = menus[i];
                break;
            }
        }
        //メニューを開いた時に項目のインデックスを０にする
        if (prevCurrentMenu != currentMenu)
        {
            menuPieceIndex = 0;
        }
        prevCurrentMenu = currentMenu;

        if (currentMenu)
        {
            if (IsUserControl())
            {
                KeyMap keyMap = GetManager().GetKeyMap();
                int playerIndex = GetManager().GetPlayerIndex();

                Vector2 stickInput = keyMap.GetVectorInput(playerIndex, "moveStick")
                    + keyMap.GetVectorInput(playerIndex, "moveStick2");
                bool selectButton = keyMap.GetKey(playerIndex, "select");
                backInput = keyMap.GetKey(playerIndex, "back");
                bool upInputButton = keyMap.GetKey(playerIndex, "up") || stickInput.y > 0;
                bool downInputButton = keyMap.GetKey(playerIndex, "down") || stickInput.y < 0;
                bool leftInputButton = keyMap.GetKey(playerIndex, "left") || stickInput.x < 0;
                bool rightInputButton = keyMap.GetKey(playerIndex, "right") || stickInput.x > 0;
                bool moveInputButton =
                    upInputButton || downInputButton
                    || leftInputButton || rightInputButton;

                prevInput = input;
                prevKeyInputVec = keyInputVec;

                //クリックしたらマウス操作に切り替え
                if (KX_netUtil.GetISMouseButton("leftButton"))
                {
                    isMouseInput = true;
                }
                //何かボタンを押したらボタン操作に切り替え
                if (selectButton
                    || backInput
                    || moveInputButton)
                {
                    isMouseInput = false;
                }

                //配列の外に出ないようにする
                menuPieceIndex =
                    menuPieceIndex % currentMenu.GetMemuPieces().Length;
                //参照を省略
                CMBMenuPiece currentMenuPiece =
                    currentMenu.GetMemuPieces()[menuPieceIndex];
                //カーソルを項目の位置に合わせる
                menuCursor.transform.position =
                        KX_netUtil.GetRectRelativePosition(
                        currentMenuPiece.GetRectTransform(),
                        new Vector2(0.5f, -0.5f));
                menuCursor2.transform.position =
                    KX_netUtil.GetRectRelativePosition(
                    currentMenuPiece.GetRectTransform(),
                    new Vector2(-0.5f, 0.5f));

                if (isMouseInput)
                {
                    //マウス操作
                    input = KX_netUtil.GetISMouseButton("leftButton");
                    if (input)
                    {
                        inputPosition = KX_netUtil.GetISMousePosition();
                    }
                }
                else
                {
                    //ボタン操作
                    input = selectButton;

                    //スライダー選択中以外は方向入力で項目選択
                    if (!menuPieceControlling)
                    {
                        keyInputVec = Vector3.zero;
                        if (upInputButton)
                        {
                            keyInputVec += new Vector3(0, 1, 0);
                        }
                        if (downInputButton)
                        {
                            keyInputVec += new Vector3(0, -1, 0);
                        }
                        if (rightInputButton)
                        {
                            keyInputVec += new Vector3(1, 0, 0);
                        }
                        if (leftInputButton)
                        {
                            keyInputVec += new Vector3(-1, 0, 0);
                        }
                        Vector3 outputKeyInputVec =
                            keyInputVec - prevKeyInputVec;

                        int holdMenuPieceIndex = menuPieceIndex;
                        if (keyInputVec != Vector3.zero
                            && outputKeyInputVec != Vector3.zero)
                        {
                            Vector2 currentMenuPiecePos =
                                RectTransformUtility.WorldToScreenPoint(
                                GetCamera(), currentMenu.GetMemuPieces()[menuPieceIndex].
                                transform.position);
                            int targetIndex = 0;
                            float targetDot = 0;
                            //入力と同じ方向にある項目へ移動
                            for (int i = 0; i < currentMenu.GetMemuPieces().Length; i++)
                            {
                                if (i != menuPieceIndex)
                                {
                                    Vector2 currentTargetMenuPiecePos =
                                        RectTransformUtility.WorldToScreenPoint(
                                        GetCamera(), currentMenu.GetMemuPieces()[i].transform.position);
                                    float currentTargetDot = Vector2.Dot(
                                        new Vector2(keyInputVec.x, keyInputVec.y).normalized,
                                        (currentTargetMenuPiecePos - currentMenuPiecePos).normalized
                                        / (currentTargetMenuPiecePos - currentMenuPiecePos).magnitude);

                                    if (i == 0 || currentTargetDot > targetDot)
                                    {
                                        targetDot = currentTargetDot;
                                        targetIndex = i;
                                    }
                                }
                            }

                            Vector2 targetMenuPiecePos =
                                RectTransformUtility.WorldToScreenPoint(
                                GetCamera(), currentMenu.GetMemuPieces()[targetIndex].transform.position);
                            targetDot = Vector2.Dot(
                                new Vector2(keyInputVec.x, keyInputVec.y).normalized,
                                (targetMenuPiecePos - currentMenuPiecePos).normalized);
                            if (targetDot > 0)
                            {
                                menuPieceIndex = targetIndex;
                            }
                        }
                    }

                    //inputPositionを項目の位置に合わせる
                    inputPosition =
                        GetCamera().WorldToScreenPoint(KX_netUtil.GetRectCenterPosition(
                        currentMenuPiece.GetRectTransform()));

                    bool holdInput = input;

                    //参照を省略
                    CMBSlider currentSlider =
                        currentMenuPiece.GetComponent<CMBSlider>();
                    //スライダー選択中
                    if (currentSlider)
                    {
                        input = false;
                        if (holdInput && !prevHoldInput)
                        {
                            menuPieceControlling = !menuPieceControlling;
                        }

                        if (menuPieceControlling)
                        {
                            int keyinputDir = 0;
                            if (rightInputButton || upInputButton)
                            {
                                keyinputDir += 1;
                            }
                            if (leftInputButton || downInputButton)
                            {
                                keyinputDir -= 1;
                            }

                            if (keyinputDir != 0)
                            {
                                input = true;
                                inputPosition =
                                    GetCamera().WorldToScreenPoint(
                                    KX_netUtil.GetRectRelativePosition(
                                    currentSlider.GetBarTransform(),
                                    new Vector2(Mathf.Lerp(0.5f, -0.5f,
                                    currentSlider.GetOutputValue()
                                    + keyinputDir * 0.01f), 0)));
                            }

                            //カーソルをスライダーの位置に合わせる
                            menuCursor.transform.position =
                                KX_netUtil.GetRectRelativePosition(
                                currentSlider.GetBarTransform(),
                                new Vector2(0.5f, -0.5f));
                            menuCursor2.transform.position =
                                KX_netUtil.GetRectRelativePosition(
                                currentSlider.GetBarTransform(),
                                new Vector2(-0.5f, 0.5f));
                        }
                    }
                    else
                    {
                        menuPieceControlling = false;
                    }
                    prevHoldInput = holdInput;
                }
            }

            if (input)
            {
                Vector3 inputPosition3D = inputPosition;
                inputPosition3D.z = GetVisualDepth();
                Vector3 screenToWorldPointPosition =
                    GetCamera().ScreenToWorldPoint(inputPosition3D);
            }
        }

        //メニュー操作時のみカーソルを表示
        menuCursor.enabled = menuCursor2.enabled = currentMenu;

        cursorPos = GetCamera().WorldToScreenPoint(
            menuCursor.transform.position);
        cursorPos.z = GetVisualDepth();

        cursorPos2 = GetCamera().WorldToScreenPoint(
            menuCursor2.transform.position);
        cursorPos2.z = GetVisualDepth();

        menuCursor.transform.position =
            GetCamera().ScreenToWorldPoint(cursorPos);
        menuCursor2.transform.position =
            GetCamera().ScreenToWorldPoint(cursorPos2);
    }

    public bool IsReleased()
    {
        return !input && prevInput;
    }
}