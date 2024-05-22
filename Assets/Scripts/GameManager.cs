using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //�@�ړ��惁�_�����\��UI
    public Text textComponent;

    static uint medalCount; // ���_���̊l������

    public void Start()
    {
        medalCount = 0;
    }

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

    // ���_���̊l��
    public void AddMedalCount()
    {
        medalCount++;
        textComponent.text = "Medals : " + medalCount;
    }
}
