using UnityEngine;

public class ControlGuide : MonoBehaviour
{
    TutorialPanel tutorialPanel;
    public TutorialPanel GetTutorialPanel()
    {
        return tutorialPanel;
    }
    TutorialPanel.ControlAnimData data;
    public TutorialPanel.ControlAnimData GetData()
    {
        return data;
    }
    TutorialPanel.AnimFrameData currentFrame;
    public TutorialPanel.AnimFrameData GetCurrentFrame()
    {
        return currentFrame;
    }
    ControlMapManager controlMapManager;
    public ControlMapManager GetControlMapManager()
    {
        return controlMapManager;
    }
    KeyMap keyMap;
    public KeyMap GetKeyMap()
    {
        return keyMap;
    }

    void FixedUpdate()
    {
        tutorialPanel = transform.parent.GetComponent<TutorialPanel>();
        if (tutorialPanel)
        {
            data = tutorialPanel.data;
            currentFrame = tutorialPanel.GetCurrentFrame();
            controlMapManager = tutorialPanel.GetControlMapManager();
            keyMap = controlMapManager.GetKeyMap();

            ControlGuideUpdate();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void ControlGuideUpdate()
    {
    }
}