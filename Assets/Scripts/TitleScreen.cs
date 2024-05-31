using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    GameObject logo;
    Vector3 logoDefaultpos;
    [SerializeField]
    GameObject startControlShadow;
    [SerializeField]
    string nextSceneName;
    [SerializeField]
    float logoWaveAmount = 0.03f;
    [SerializeField]
    float logoWaveSpeed = 1;
    [SerializeField]
    float startControlShadowRadius = 0.07f;
    [SerializeField]
    float startControlShadowSpeed = 1.5f;

    void Awake()
    {
        logoDefaultpos = logo.transform.localPosition;
    }
    void FixedUpdate()
    {
        logo.transform.localPosition =
            logoDefaultpos + new Vector3(0,
            Mathf.Sin(Time.time * logoWaveSpeed) * logoWaveAmount, 0);
        startControlShadow.transform.localPosition =
            new Vector3(Mathf.Sin(Time.time * startControlShadowSpeed),
            Mathf.Cos(Time.time * startControlShadowSpeed), 0)
            * startControlShadowRadius;

        bool screenTap = Input.GetKey(KeyCode.Space)
        || Input.GetKey(KeyCode.Joystick1Button0) || Input.GetKey(KeyCode.Joystick1Button1)
        || Input.GetKey(KeyCode.Joystick1Button2) || Input.GetKey(KeyCode.Joystick1Button3)
        || Input.GetKey(KeyCode.Mouse0);

        if (screenTap)
        {
            SceneTransition.ChangeScene(nextSceneName);
        }
    }
}
