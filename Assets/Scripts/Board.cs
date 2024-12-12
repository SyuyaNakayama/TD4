using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // 表示テキスト
    public GameObject textComponent;

    void Start()
    {
        textComponent.SetActive(false);
    }

    void FixedUpdate()
    {
        textComponent.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        LiveEntity liveEntity = other.GetComponent<LiveEntity>();
        // プレイヤーが接触したらメッセージを表示する
        if (liveEntity && liveEntity.GetUserControl())
        {
            textComponent.SetActive(true);
        }
    }
}
