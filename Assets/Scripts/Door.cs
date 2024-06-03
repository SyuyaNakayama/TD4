using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    string nextScene;
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.GetComponent<Player>() != null)
        {
            SceneTransition.ChangeScene(nextScene);
        }
    }
}