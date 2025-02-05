using UnityEngine;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    ControlMapPlayPart controlMapPlayPart;
    [SerializeField]
    GameObject logo;
    Vector3 logoDefaultpos;
    [SerializeField]
    GameObject startControlShadow;
    [SerializeField]
    TMP_Text versionText;
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
        versionText.text = "ver " + Application.version;

        logo.transform.localPosition =
            logoDefaultpos + new Vector3(0,
            Mathf.Sin(Time.time * logoWaveSpeed) * logoWaveAmount, 0);
        startControlShadow.transform.localPosition =
            new Vector3(Mathf.Sin(Time.time * startControlShadowSpeed),
            Mathf.Cos(Time.time * startControlShadowSpeed), 0)
            * startControlShadowRadius;

        if (controlMapPlayPart.GetJumpInput()
            || controlMapPlayPart.GetWeaponInput())
        {
            SceneTransition.ChangeScene(nextSceneName);
        }
    }
}
