using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ControlMap : MonoBehaviour
{
    const int visualDepthMultiply = 6;

    [SerializeField]
    LiveEntity liveEntity;
    public LiveEntity GetLiveEntity()
    {
        return liveEntity;
    }
    [SerializeField]
    Camera camera;
    public Camera GetCamera()
    {
        return camera;
    }
    [SerializeField]
    KeyBinder[] keyBinders = { };
    [SerializeField]
    KeyMap keyMap;
    protected KeyMap GetKeyMap()
    {
        return keyMap;
    }
    [SerializeField]
    bool userControl;

    protected Vector2 inputPosition;
    public Vector2 GetInputPosition()
    {
        return inputPosition;
    }

    void FixedUpdate()
    {
        if (IsUserControl())
        {
            if (keyMap == null)
            {
                Load();
            }
            Save();
        }

        ControlMapUpdate();
    }

    public float GetVisualDepth()
    {
        return liveEntity.transform.localScale.z * visualDepthMultiply;
    }

    public bool IsUserControl()
    {
        return (liveEntity && liveEntity.GetUserControl())
            || (!liveEntity && userControl);
    }

    void Load()
    {
        keyMap = new KeyMap();

        string path = Application.persistentDataPath + "/keyMapData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            KeyMap data = JsonUtility.FromJson<KeyMap>(json);
        }
    }
    void Save()
    {

    }

    protected virtual void ControlMapUpdate()
    {
    }
}