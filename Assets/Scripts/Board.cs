using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // 表示テキスト
    public GameObject textComponent;

    // Start is called before the first frame update
    void Start()
    {
        textComponent.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        textComponent.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            textComponent.SetActive(true);
        }
    }
}
