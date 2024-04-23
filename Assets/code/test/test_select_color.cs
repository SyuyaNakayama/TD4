using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_select_color : MonoBehaviour
{
    public GameObject testObj;
    private test_select tSel;

    private int stageNum = 0;

    void Start()
    {
        tSel = testObj.GetComponent<test_select>();
    }

    void Update()
    {
        stageNum = tSel.GetSelectNum();
        switch (stageNum)
        {
            case 0:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 0);
                break;
            case 1:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(0, 255, 0, 0);
                break;
            case 2:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(0, 0, 255, 0);
                break;
            default:
                this.gameObject.GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 0);
                break;
        }
    }
}
