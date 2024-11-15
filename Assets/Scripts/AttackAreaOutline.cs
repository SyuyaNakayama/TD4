using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaOutline : MonoBehaviour
{
    [SerializeField]
    AttackArea attackArea;
    [SerializeField]
    MeshRenderer meshRenderer;
    [SerializeField]
    int materialIndex;

    void OnWillRenderObject()
    {
        if (attackArea && attackArea.GetAttacker()
        && meshRenderer && meshRenderer.materials.Length > materialIndex)
        {
            Color outlineColor =
                attackArea.GetAttacker().GetCassetteData().GetThemeColor();
            outlineColor.a = 1;
            meshRenderer.materials[materialIndex].SetColor(
                "_OutlineColor", outlineColor);
        }
    }
}
