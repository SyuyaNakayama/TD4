using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // �\���e�L�X�g
    public GameObject textComponent;

    // Start is called before the first frame update
    void Start()
    {
        textComponent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) { textComponent.SetActive(false); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") { return; }
        textComponent.SetActive(true);
    }
}
