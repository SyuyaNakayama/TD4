using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour
{
    public Shader shader;
    private Material material;

    private void Awake()
    {
        material = new Material(shader); // shaderを割り当てたマテリアルの動的生成
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }
}
