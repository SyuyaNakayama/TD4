using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlMapManager : MonoBehaviour
{
    [Serializable]
    public enum InputDevice
    {
        keyboard,
        mouse,
        gamepad,
    }
    [Serializable]
    public struct TaggedControlIcons
    {
        public InputDevice tag;
        public ControlIcons controlIcons;
    }
    [Serializable]
    public struct TaggedControlIconAtlas
    {
        public InputDevice tag;
        public ControlIconAtlas controlIconAtlas;
    }

    [Serializable]
    public struct PlayerInputDevice
    {
        public bool keyboard;
        public bool mouse;
        public int[] gamepads;

        public PlayerInputDevice(bool setKeyboard,
            bool setMouse, int[] setGamepads)
        {
            keyboard = setKeyboard;
            mouse = setMouse;
            gamepads = KX_netUtil.CopyArray(setGamepads);
        }
    }

    [SerializeField]
    static PlayerInputDevice[] players = {
        new PlayerInputDevice(true, true, new int[] { 0 }) };
    public static PlayerInputDevice[] GetPlayers()
    {
        PlayerInputDevice[] ret =
            KX_netUtil.CopyArray(players);
        for (int i = 0; i < ret.Length; i++)
        {
            ret[i].gamepads =
                KX_netUtil.CopyArray(players[i].gamepads);
        }
        return ret;
    }

    [SerializeField]
    LiveEntity liveEntity;
    public LiveEntity GetLiveEntity()
    {
        return liveEntity;
    }
    [SerializeField]
    string keyMapDataName;
    [SerializeField]
    KeyBinder[] keyBinders = { };
    [SerializeField]
    VectorInputBinder[] vectorInputBinders = { };
    [SerializeField]
    CMBButton resetButton;
    [SerializeField]
    KeyMap defaultKeyMap;
    [SerializeField]
    TaggedControlIcons[] controlIcons = { };
    [SerializeField]
    TaggedControlIconAtlas[] defaultControlIconAtlas = { };
    [SerializeField]
    InputDevice[] allowedInputDevices =
        { InputDevice.keyboard,InputDevice.mouse,InputDevice.gamepad};
    [SerializeField]
    bool userControl;
    [SerializeField]
    int playerIndex;

    KeyMap keyMap;
    public KeyMap GetKeyMap()
    {
        if (keyMap)
        {
            return Instantiate(keyMap);
        }
        return null;
    }
    TaggedControlIconAtlas[] controlIconAtlas = { };
    InputDevice latestInputDevice;
    public InputDevice GetLatestInputDevice()
    {
        return latestInputDevice;
    }

    void Awake()
    {
        keyMap = Instantiate(defaultKeyMap);

        controlIconAtlas =
            KX_netUtil.CopyArray(defaultControlIconAtlas);
        for (int i = 0; i < controlIconAtlas.Length; i++)
        {
            controlIconAtlas[i].controlIconAtlas =
                Instantiate(defaultControlIconAtlas[i].controlIconAtlas);
        }

        if (IsUserControl())
        {
            Load();
        }
    }

    void FixedUpdate()
    {
        if (!keyMap)
        {
            keyMap = Instantiate(defaultKeyMap);
        }
        if (controlIconAtlas == null)
        {
            controlIconAtlas =
                KX_netUtil.CopyArray(defaultControlIconAtlas);
            for (int i = 0; i < controlIconAtlas.Length; i++)
            {
                controlIconAtlas[i].controlIconAtlas =
                    Instantiate(defaultControlIconAtlas[i].controlIconAtlas);
            }
        }

        if (IsUserControl())
        {
            //入力されたデバイスに応じて変化
            if (KX_netUtil.ISAnyKey()
                && Array.IndexOf(allowedInputDevices, InputDevice.keyboard) >= 0)
            {
                latestInputDevice = InputDevice.keyboard;
            }
            if (KX_netUtil.GetISMouseButton("leftButton")
                && Array.IndexOf(allowedInputDevices, InputDevice.mouse) >= 0)
            {
                latestInputDevice = InputDevice.mouse;
            }
            if (KX_netUtil.ISAnyPadButton(GetPlayers()[playerIndex].gamepads[0])
                && Array.IndexOf(allowedInputDevices, InputDevice.gamepad) >= 0)
            {
                latestInputDevice = InputDevice.gamepad;
            }

            //リセットボタンが押されたらキーバインドをリセットする
            if (resetButton && resetButton.GetOutput())
            {
                keyMap = Instantiate(defaultKeyMap);
            }
            Save();
        }
    }

    void Load()
    {
        string path = Application.persistentDataPath
            + "/" + keyMapDataName + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, keyMap);
        }
    }
    void Save()
    {
        string json = JsonUtility.ToJson(keyMap, true);
        File.WriteAllText(Application.persistentDataPath
            + "/" + keyMapDataName + ".json", json);
    }

    public bool IsUserControl()
    {
        return (liveEntity && liveEntity.GetUserControl())
            || (!liveEntity && userControl);
    }
    public int GetPlayerIndex()
    {
        return playerIndex;
    }

    public TaggedControlIcons SearchControlIcons(InputDevice tag)
    {
        //同じタグのものがあったらそれを返す
        for (int i = 0; i < controlIcons.Length; i++)
        {
            if (controlIcons[i].tag == tag)
            {
                return controlIcons[i];
            }
        }
        //無ければ空の要素を返す
        return new TaggedControlIcons();
    }

    public TaggedControlIconAtlas SearchControlIconAtlas(InputDevice tag)
    {
        //同じタグのものがあったらそれを返す
        for (int i = 0; i < controlIconAtlas.Length; i++)
        {
            if (controlIconAtlas[i].tag == tag)
            {
                return controlIconAtlas[i];
            }
        }
        //無ければ空の要素を返す
        return new TaggedControlIconAtlas();
    }

    public void ApplyKeyBind()
    {
        for (int i = 0; i < keyBinders.Length; i++)
        {
            if (keyBinders[i].IsCurrentMenu())
            {
                if (keyBinders[i].GetBindKeys().Length > 0)
                {
                    keyMap.SetKeyMap(
                        keyBinders[i].GetKeyMapCellName(),
                        keyBinders[i].GetBindKeys());
                }
                if (keyBinders[i].GetBindButtons().Length > 0)
                {
                    keyMap.SetKeyMap(
                        keyBinders[i].GetKeyMapCellName(),
                        keyBinders[i].GetBindButtons());
                }
            }
        }
    }

    public void ApplyVecBind()
    {
        for (int i = 0; i < vectorInputBinders.Length; i++)
        {
            if (vectorInputBinders[i].IsCurrentMenu())
            {
                keyMap.SetVectorInputMap(
                    vectorInputBinders[i].GetVecCellName(),
                    vectorInputBinders[i].GetAxisBindData());
            }
        }
    }
}