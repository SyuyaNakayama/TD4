using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medal : MonoBehaviour
{
    public GameManager gameManager;
    private SaveMedals saveMedals;
    //�����ڂ�
    public int medalNum;

    // Start is called before the first frame update
    void Start()
    {
        GameObject managerObject = GameObject.Find("GameManager");
        gameManager = managerObject.GetComponent<GameManager>();
        saveMedals = managerObject.GetComponent<SaveMedals>();
    }

    // Update is called once per frame
    void Update()
    {
        if (saveMedals.GetMedalData(medalNum))
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        // ���肪�v���C���[�̎��̂ݏ���
        if (collision.gameObject.tag != "Player") { return; }

        // ���_�����l������
        saveMedals.AddMedalCount();
        saveMedals.AcquisitionMedal(medalNum);
        Destroy(gameObject);
    }
}
