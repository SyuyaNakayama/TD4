using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPasteManager : MonoBehaviour
{
    [SerializeField]
    int matIndex;
    public bool squareTiling;
    public Vector2 tiling;
    public Vector2 offset;
    public float alpha;

    void OnWillRenderObject()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material mat = renderer.materials[matIndex];

        if(mat != null)
        {
            Vector2 tile = tiling;
            if(squareTiling)
            {
                tile.x *= Screen.width / (float)Screen.height;
            }
            mat.SetTextureScale("_MainTex", tile);
            mat.SetTextureOffset("_MainTex", offset);
            mat.SetFloat("_MainAlpha", alpha);
        }
    }
}
