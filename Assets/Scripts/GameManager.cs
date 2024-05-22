using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //　移動先メダル数表示UI
    public Text textComponent;

    static uint medalCount; // メダルの獲得枚数

    public void Start()
    {
        medalCount = 0;
    }

    //シーンリセット
    public void SceneReset()
    {
        string acticeSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(acticeSceneName);
    }

    //シーンチェンジ
    public void ChangeScene(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }

    // メダルの獲得
    public void AddMedalCount()
    {
        medalCount++;
        textComponent.text = "Medals : " + medalCount;
    }
}
