using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_select : MonoBehaviour
{
    public GameObject gmObj;
    private GameManager gm;
    private string sceneName = "test_stage0";
    private int stageNum = 0;
    private const int minStageNum = 0;
    private const int maxStageNum = 2;

    private void Start()
    {
        gm = gmObj.GetComponent<GameManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            stageNum--;
            if(stageNum < minStageNum)
            {
                stageNum = maxStageNum;
            }
            ChangeTargetStage();
            Debug.Log("test_stage" +  stageNum);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            stageNum++;
            if (stageNum > maxStageNum)
            {
                stageNum = minStageNum;
            }
            ChangeTargetStage();
            Debug.Log("test_stage" + stageNum);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gm.ChangeScene(sceneName);
        }
    }
    private void ChangeTargetStage()
    {
        sceneName = "test_stage" + stageNum;
    }
    public int GetSelectNum()
    {
        Debug.Log(stageNum);
        return stageNum;
    }
}
