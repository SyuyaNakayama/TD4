using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medal : MonoBehaviour
{
    public GameManager gameManager;
    //何枚目か
    public int MedalNum;
    //取得しているかどうか
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
        // 相手がプレイヤーの時のみ処理
        if (collision.gameObject.tag != "Player") { return; }

        // メダルを獲得した
        gameManager.AddMedalCount();
        Destroy(gameObject);
    }
}
