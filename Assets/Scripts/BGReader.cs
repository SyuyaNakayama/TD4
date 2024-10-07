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
        if (StageManager.GetCurrent())
        {
            image.sprite = StageManager.GetCurrent().GetBackGround();
        }
    }
}
