using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    string nextScene;

    void OnWillRenderObject()
    {
        Vector2 tiling = new Vector2(8 * Screen.width / (float)Screen.height, 8);
        Vector2 offset = new Vector2(Mathf.Repeat(Time.time / 8, 1), Mathf.Repeat(Time.time / 8, 1));
        GetComponent<Renderer>().materials[0].SetTextureScale("_MainTex", tiling);
        GetComponent<Renderer>().materials[0].SetTextureOffset("_MainTex", offset);

        GetComponent<Renderer>().materials[0].SetFloat("_MainAlpha", 0.5f);
    }
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.GetComponent<LiveEntity>() != null)
        {
            SceneTransition.ChangeScene(nextScene);
        }
    }
}