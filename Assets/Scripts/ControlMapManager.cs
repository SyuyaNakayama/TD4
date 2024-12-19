using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ControlMapManager : MonoBehaviour
{
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
    KeyMap defaultKeyMap;
    [SerializeField]
    bool userControl;

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

    public void ApplyKeyBind()
    {
        for (int i = 0; i < keyBinders.Length; i++)
        {
            if (keyBinders[i].IsCurrentMenu())
            {
                keyMap.SetKeyMap(
                    keyBinders[i].GetKeyMapCellName(),
                    keyBinders[i].GetBindKeys());
            }
        }
    }

    public void ApplyVecBind()
    {
        for (int i = 0; i < vectorInputBinders.Length; i++)
        {
            if (vectorInputBinders[i].IsCurrentMenu())
            {

            }
        }
    }
}