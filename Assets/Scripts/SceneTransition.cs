using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    const float sceneChangeSpeed = 0.05f;

    [SerializeField]
    float shutterScale;
    static bool sceneChangeSwitch = false;
    public static bool GetSceneChangeSwitch()
    {
        return sceneChangeSwitch;
    }
    static float sceneChangeProgress = 0;
    public static float GetSceneChangeProgress()
    {
        return sceneChangeProgress;
    }
    static string nextScene;

    void Awake()
    {
        sceneChangeProgress = 1;
        sceneChangeSwitch = false;
        transform.localScale = new Vector3(1, 1, 1) * shutterScale;
    }
    void FixedUpdate()
    {
        float scale = Mathf.Sin(Mathf.PI / 2 * sceneChangeProgress);
        transform.localScale = new Vector3(1, 1, 1) * scale * shutterScale;

        if (sceneChangeSwitch)
        {
            sceneChangeProgress += sceneChangeSpeed;

            if (sceneChangeProgress >= 1)
            {
                SceneManager.LoadScene(nextScene);
            }
        }
        else
        {
            if (sceneChangeProgress >= 1)
            {
                sceneChangeProgress =
                    Mathf.Repeat(sceneChangeProgress + sceneChangeSpeed, 2);
            }
            else
            {
                sceneChangeProgress = 0;
            }
        }
    }
    static public void ChangeScene(string setNextScene)
    {
        nextScene = setNextScene;
        sceneChangeSwitch = true;
    }
}