using UnityEngine;

public class Switch : MonoBehaviour
{
    protected const int maxPushCoolTimeFrame = 5;

    [SerializeField]
    protected Sprite pushed;
    protected int pushCoolTimeFrame;
    protected bool active;
    public bool GetActive()
    {
        return active;
    }
}
