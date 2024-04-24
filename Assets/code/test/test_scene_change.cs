using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_scene_change : MonoBehaviour
{
    public GameObject gmObj;
    private GameManager gm;
    public string sceneName;

    private void Start()
    {
        gm = gmObj.GetComponent<GameManager>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gm.ChangeScene(sceneName);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            gm.ChangeScene("SampleScene");
        }
    }
}
