using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medal : MonoBehaviour
{
    public GameManager gameManager;
    private SaveMedals saveMedals;
    //何枚目か
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
        transform.Rotate(new Vector3(90, 0, 0) * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider col)
    {
        // 相手がプレイヤーの時のみ処理
        if (col.gameObject.GetComponent<Player>() != null)
        {
            // メダルを獲得した
            saveMedals.AddMedalCount();
            saveMedals.AcquisitionMedal(medalNum);
            Destroy(gameObject);
        }
    }
}
