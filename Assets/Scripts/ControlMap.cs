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
    ControlMapManager manager;
    public ControlMapManager GetManager()
    {
        return manager;
    }

    protected Vector2 inputPosition;
    public Vector2 GetInputPosition()
    {
        return inputPosition;
    }

    void FixedUpdate()
    {
        ControlMapUpdate();
    }

    public float GetVisualDepth()
    {
        return liveEntity.transform.localScale.z * visualDepthMultiply;
    }

    public bool IsUserControl()
    {
        return GetManager().IsUserControl();
    }

    protected virtual void ControlMapUpdate()
    {
    }
}