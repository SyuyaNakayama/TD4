using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CMBSlider : CMBMenuPiece
{
    [SerializeField]
    Image bar;
    [SerializeField]
    RectTransform barTransform;
    public RectTransform GetBarTransform()
    {
        return barTransform;
    }
    [SerializeField]
    Image slider;
    [SerializeField]
    RectTransform sliderTransform;

    [SerializeField]
    Vector2 range = new Vector2(0, 1);
    [SerializeField, Range(0, 1)]
    float amount;

    protected override void MPUpdate()
    {
        if (GetControlMap().GetInput() &&
            KX_netUtil.IsInsideHitBox(
                barTransform,
                GetControlMap().GetLiveEntity().GetView(),
                GetControlMap().GetInputPosition()))
        {
            output = true;
            amount = KX_netUtil.RangeMap(Mathf.Clamp(
                KX_netUtil.InverseTransformPointHitBox(
                barTransform,
                GetControlMap().GetLiveEntity().GetView(),
                GetControlMap().GetInputPosition()).x,
                -0.5f, 0.5f), -0.5f, 0.5f, 0, 1);
        }

        sliderTransform.anchorMin = new Vector2(amount - 0.05f, -0.1f);
        sliderTransform.anchorMax = new Vector2(amount + 0.05f, 1.1f);
    }

    public float GetScaledOutputValue()
    {
        return KX_netUtil.RangeMap(amount, 0, 1, range.x, range.y);
    }

    public override float GetOutputValue()
    {
        return amount;
    }

    protected override void MPLoad(SaveData.NameAndFloat nameAndFloat)
    {
        amount = nameAndFloat.value;
    }
}
