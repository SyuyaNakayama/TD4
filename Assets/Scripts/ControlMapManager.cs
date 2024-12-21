using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlMapManager : MonoBehaviour
{
    [System.Serializable]
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
            gamepads = KX_netUtil.CopyArray<int>(setGamepads);
        }
    }

    [SerializeField]
    static PlayerInputDevice[] players = {
        new PlayerInputDevice(true, true, new int[] { 0 }) };
    public static PlayerInputDevice[] GetPlayers()
    {
        PlayerInputDevice[] ret =
            KX_netUtil.CopyArray<PlayerInputDevice>(players);
        for (int i = 0; i < ret.Length; i++)
        {
            ret[i].gamepads =
                KX_netUtil.CopyArray<int>(players[i].gamepads);
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
    bool userControl;
    [SerializeField]
    int playerIndex;

    KeyMap keyMap;
    public KeyMap GetKeyMap()
    {
        return Instantiate(keyMap);
    }

    void Awake()
    {
        if (IsUserControl())
        {
            keyMap = Instantiate(defaultKeyMap);
            Load();
        }
    }

    void FixedUpdate()
    {
        if (IsUserControl())
        {
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