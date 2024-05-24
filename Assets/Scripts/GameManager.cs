using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //�V�[�����Z�b�g
    public void SceneReset()
    {
        string acticeSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(acticeSceneName);
    }

    //�V�[���`�F���W
    public void ChangeScene(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }
}
