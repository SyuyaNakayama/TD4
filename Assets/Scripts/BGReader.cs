using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGReader : MonoBehaviour
{
    [SerializeField]
    Image image;

    StageManager stageManager;

    void FixedUpdate()
    {
        if (stageManager == null)
        {
            //ステージマネージャーを探す
            foreach (StageManager obj in UnityEngine.Object.FindObjectsOfType<StageManager>())
            {
                stageManager = obj;
            }
        }
        image.sprite = stageManager.GetBackGround();
    }
}
