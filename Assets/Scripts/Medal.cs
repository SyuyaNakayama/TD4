using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medal : MonoBehaviour
{
    public GameManager gameManager;
    //�����ڂ�
    public int MedalNum;
    //�擾���Ă��邩�ǂ���
    private bool isGet = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject managerObject = GameObject.Find("GameManager");
        gameManager = managerObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        // ���肪�v���C���[�̎��̂ݏ���
        if (collision.gameObject.tag != "Player") { return; }

        // ���_�����l������
        gameManager.AddMedalCount();
        Destroy(gameObject);
    }
}
