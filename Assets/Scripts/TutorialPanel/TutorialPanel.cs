using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class TutorialPanel : MonoBehaviour
{
    const float controlGuideDistance = 0.3f;

    [Serializable]
    public struct ButtonAnimKey
    {
        public Vector2 keyFrame;
        public int buttonInputIndex;
    }
    [Serializable]
    public struct DirectionAnimKey
    {
        public Vector2 keyFrame;
        public bool upInput;
        public bool downInput;
        public bool leftInput;
        public bool rightInput;
    }
    [Serializable]
    public struct ControlAnimData
    {
        public string displayName;
        public string bindActName;
        public int totalFrame;
        public string[] useButtonInputNames;
        public ButtonAnimKey[] buttonAnimKeys;
        public DirectionAnimKey[] directionAnimKeys;
    }

    public struct AnimFrameData
    {
        public string[] pressedButtonInputNames;
        public Vector2Int directionInput;
    }

    public ControlAnimData data;

    [SerializeField]
    ControlMapManager controlMapManager;
    public ControlMapManager GetControlMapManager()
    {
        return controlMapManager;
    }
    [SerializeField]
    TMP_Text text;
    [SerializeField]
    TouchGuide touchGuide;
    [SerializeField]
    KeyGuide keyGuide;
    [SerializeField]
    ButtonGuide buttonGuide;
    [SerializeField]
    DirectionInputGuide directionInputGuide;
    [SerializeField]
    Vector3 textPos;
    [SerializeField]
    Vector3 iconPos;
    [SerializeField]
    bool isLeftExtend;

    int frame;
    float progress;
    public float GetProgress()
    {
        return progress;
    }
    float prevProgress;
    public float GetPrevProgress()
    {
        return prevProgress;
    }
    AnimFrameData currentFrame;
    public AnimFrameData GetCurrentFrame()
    {
        return currentFrame;
    }
    TouchGuide[] touchGuides = { };
    public TouchGuide[] GetTouchGuides()
    {
        return KX_netUtil.CopyArray(touchGuides);
    }
    KeyGuide[] keyGuides = { };
    public KeyGuide[] GetKeyGuides()
    {
        return KX_netUtil.CopyArray(keyGuides);
    }
    ButtonGuide[] buttonGuides = { };
    public ButtonGuide[] GetButtonGuides()
    {
        return KX_netUtil.CopyArray(buttonGuides);
    }
    DirectionInputGuide[] directionInputGuides = { };
    public DirectionInputGuide[] GetDirectionInputGuides()
    {
        return KX_netUtil.CopyArray(directionInputGuides);
    }

    void FixedUpdate()
    {
        text.transform.localPosition = textPos;
        text.text = data.displayName;
        //フレームを一つ進める
        frame = Mathf.RoundToInt(Mathf.Repeat(frame + 1, data.totalFrame));
        prevProgress = progress;
        progress = (float)frame / data.totalFrame;
        //アニメーション内の今のフレームの入力状態を取得
        currentFrame = UpdateFrameData();

        //nullを削除し、一応重複する要素も探して削除
        List<TouchGuide> TGsList =
            new List<TouchGuide>(touchGuides);
        TGsList.RemoveAll(where => !where);
        TGsList = TGsList.Distinct().ToList();
        touchGuides = TGsList.ToArray();

        List<DirectionInputGuide> DIGsList =
            new List<DirectionInputGuide>(directionInputGuides);
        DIGsList.RemoveAll(where => !where);
        DIGsList = DIGsList.Distinct().ToList();
        directionInputGuides = DIGsList.ToArray();

        List<KeyGuide> KGsList =
            new List<KeyGuide>(keyGuides);
        KGsList.RemoveAll(where => !where);
        KGsList = KGsList.Distinct().ToList();
        keyGuides = KGsList.ToArray();

        List<ButtonGuide> BGsList =
            new List<ButtonGuide>(buttonGuides);
        BGsList.RemoveAll(where => !where);
        BGsList = BGsList.Distinct().ToList();
        buttonGuides = BGsList.ToArray();

        int TouchGuideLength = 0;
        int KeyGuideLength = 0;
        int ButtonGuideLength = 0;
        int directionInputGuideLength = 0;

        //ControlMapManagerを探す
        LiveEntity liveEntity = transform.parent.GetComponent<LiveEntity>();

        if (!controlMapManager)
        {
            controlMapManager =
                liveEntity.GetControlMap().GetManager();
        }

        if (controlMapManager)
        {
            //最後に入力したデバイスに応じて変化
            switch (controlMapManager.GetLatestInputDevice())
            {
                //マウス
                case ControlMapManager.InputDevice.mouse:
                    //TouchGuideを生成
                    if (touchGuides.Length < 1)
                    {
                        TouchGuide current = Instantiate(
                        touchGuide.gameObject,
                        transform.position, transform.rotation, transform).
                        GetComponent<TouchGuide>();
                        //管理用配列に追加
                        Array.Resize(ref touchGuides, touchGuides.Length + 1);
                        touchGuides[touchGuides.Length - 1] = current;
                    }
                    //アイコン数を加算
                    TouchGuideLength++;
                    break;
                //キーボード
                case ControlMapManager.InputDevice.keyboard:
                    //DirectionInputGuideを生成
                    if (data.directionAnimKeys != null
                        && data.directionAnimKeys.Length > 0)
                    {
                        if (directionInputGuides.Length < 1)
                        {
                            DirectionInputGuide current = Instantiate(
                                directionInputGuide.gameObject,
                                transform.position, transform.rotation, transform).
                                GetComponent<DirectionInputGuide>();
                            //管理用配列に追加
                            Array.Resize(ref directionInputGuides, directionInputGuides.Length + 1);
                            directionInputGuides[directionInputGuides.Length - 1] = current;
                        }
                        //アイコン数を加算
                        directionInputGuideLength++;
                    }

                    //KeyGuideを生成
                    for (int i = 0; i < data.useButtonInputNames.Length; i++)
                    {
                        if (keyGuides.Length < data.useButtonInputNames.Length)
                        {
                            KeyGuide current = Instantiate(
                                keyGuide.gameObject,
                                transform.position, transform.rotation, transform).
                            GetComponent<KeyGuide>();
                            //管理用配列に追加
                            Array.Resize(ref keyGuides, keyGuides.Length + 1);
                            keyGuides[keyGuides.Length - 1] = current;
                        }
                        //アイコン数を加算
                        KeyGuideLength++;
                    }
                    break;
                //ゲームパッド
                default:
                    //DirectionInputGuideを生成
                    if (data.directionAnimKeys != null
                        && data.directionAnimKeys.Length > 0)
                    {
                        if (directionInputGuides.Length < 1)
                        {
                            DirectionInputGuide current = Instantiate(
                                directionInputGuide.gameObject,
                                transform.position, transform.rotation, transform).
                                GetComponent<DirectionInputGuide>();
                            //管理用配列に追加
                            Array.Resize(ref directionInputGuides, directionInputGuides.Length + 1);
                            directionInputGuides[directionInputGuides.Length - 1] = current;
                        }
                        //アイコン数を加算
                        directionInputGuideLength++;
                    }

                    //ButtonGuideを生成
                    for (int i = 0; i < data.useButtonInputNames.Length; i++)
                    {
                        if (buttonGuides.Length < data.useButtonInputNames.Length)
                        {
                            ButtonGuide current = Instantiate(
                                buttonGuide.gameObject,
                                transform.position, transform.rotation, transform).
                            GetComponent<ButtonGuide>();
                            //管理用配列に追加
                            Array.Resize(ref buttonGuides, buttonGuides.Length + 1);
                            buttonGuides[buttonGuides.Length - 1] = current;
                        }
                        //アイコン数を加算
                        ButtonGuideLength++;
                    }
                    break;
            }
        }

        //使うボタンなどの数と同じにする
        Array.Resize(ref touchGuides, TouchGuideLength);
        Array.Resize(ref directionInputGuides, directionInputGuideLength);
        Array.Resize(ref keyGuides, KeyGuideLength);
        Array.Resize(ref buttonGuides, ButtonGuideLength);

        Transform[] guideTransforms = { };
        Array.Resize(ref guideTransforms,
            touchGuides.Length + directionInputGuides.Length
            + keyGuides.Length + buttonGuides.Length);
        //transformを抽出
        int guideIndex = 0;
        for (int i = 0; i < touchGuides.Length; i++)
        {
            guideTransforms[guideIndex] = touchGuides[i].transform;
            guideIndex++;
        }
        for (int i = 0; i < directionInputGuides.Length; i++)
        {
            guideTransforms[guideIndex] = directionInputGuides[i].transform;
            guideIndex++;
        }
        for (int i = 0; i < keyGuides.Length; i++)
        {
            guideTransforms[guideIndex] = keyGuides[i].transform;
            guideIndex++;
        }
        for (int i = 0; i < buttonGuides.Length; i++)
        {
            guideTransforms[guideIndex] = buttonGuides[i].transform;
            guideIndex++;
        }
        //ControlGuideを並べる
        guideIndex = 0;
        if (isLeftExtend)
        {
            for (int i = guideTransforms.Length - 1; i >= 0; i++)
            {
                guideTransforms[i].localPosition =
                    iconPos + new Vector3(-guideIndex, 0, 0) * controlGuideDistance;
                guideIndex++;
            }
        }
        else
        {
            for (int i = 0; i < guideTransforms.Length; i++)
            {
                guideTransforms[i].localPosition =
                    iconPos + new Vector3(guideIndex, 0, 0) * controlGuideDistance;
                guideIndex++;
            }
        }
    }

    //アニメーションデータと進行度からフレームを取得
    AnimFrameData UpdateFrameData()
    {
        AnimFrameData ret = new AnimFrameData();
        ret.pressedButtonInputNames = new string[] { };
        ret.directionInput = Vector2Int.zero;

        //ボタン
        ButtonAnimKey[] buttonAnimKeys = data.buttonAnimKeys;
        if (buttonAnimKeys != null)
        {
            for (int i = 0; i < buttonAnimKeys.Length; i++)
            {
                ButtonAnimKey current = buttonAnimKeys[i];
                if (IsHitKeyPoint(current.keyFrame))
                {
                    Array.Resize(ref ret.pressedButtonInputNames,
                        ret.pressedButtonInputNames.Length + 1);
                    ret.pressedButtonInputNames[ret.pressedButtonInputNames.Length - 1] =
                        data.useButtonInputNames[current.buttonInputIndex];
                }
            }
        }
        //方向入力
        DirectionAnimKey[] directionAnimKeys = data.directionAnimKeys;
        if (directionAnimKeys != null)
        {
            for (int i = 0; i < directionAnimKeys.Length; i++)
            {
                DirectionAnimKey current = directionAnimKeys[i];
                if (IsHitKeyPoint(current.keyFrame))
                {
                    Vector2Int directionVec = Vector2Int.zero;
                    if (current.rightInput)
                    {
                        directionVec.x += 1;
                    }
                    if (current.leftInput)
                    {
                        directionVec.x -= 1;
                    }
                    if (current.upInput)
                    {
                        directionVec.y += 1;
                    }
                    if (current.downInput)
                    {
                        directionVec.y -= 1;
                    }
                    ret.directionInput = directionVec;
                }
            }
        }

        return ret;
    }

    //progressが指定の範囲内、もしくはその範囲を1フレーム内で通過したか
    protected bool IsHitKeyPoint(Vector2 keyPoint)
    {
        return KX_netUtil.IsCrossingRange(
            prevProgress, progress,
            keyPoint.x, keyPoint.y,
            false, false);
    }
}
