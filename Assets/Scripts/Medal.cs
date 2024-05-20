using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medal : MonoBehaviour
{
    public GameManager gameManager;

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
