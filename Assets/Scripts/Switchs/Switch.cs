using UnityEngine;

public class Switch : MonoBehaviour
{
    protected const int maxPushCoolTimeFrame = 5;

    [SerializeField]
    SpriteRenderer visual;
    public SpriteRenderer GetVisual()
    {
        return visual;
    }
    [SerializeField]
    Sprite pushed;
    public Sprite GetPushed()
    {
        return pushed;
    }
    protected int pushCoolTimeFrame;
    protected bool active;
    public bool GetActive()
    {
        return active;
    }
}
