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
    void FixedUpdate()
    {
        textComponent.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        LiveEntity liveEntity = other.GetComponent<LiveEntity>();
        // �v���C���[���ڐG�����烁�b�Z�[�W��\������
        if (liveEntity && liveEntity.GetUserControl())
        {
            textComponent.SetActive(true);
        }
    }
}
