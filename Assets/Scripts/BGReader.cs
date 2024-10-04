using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGReader : MonoBehaviour
{
    [SerializeField]
    Image image;

    void FixedUpdate()
    {
        image.sprite = StageManager.GetCurrent().GetBackGround();
    }
}
