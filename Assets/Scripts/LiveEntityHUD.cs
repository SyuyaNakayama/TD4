using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LiveEntityHUD : MonoBehaviour
{
    [SerializeField]
    LiveEntity liveEntity;
    [SerializeField]
    SpriteRenderer lifeGauge;
    [SerializeField]
    TMP_Text name;

    void FixedUpdate()
    {
        //体力ゲージを更新
        Color gaugeColor = liveEntity.GetData().GetThemeColor();
        gaugeColor.a = 1;
        lifeGauge.material.SetColor("_GaugeColor1", gaugeColor);
        lifeGauge.material.SetColor("_GaugeColor2",
            KX_netUtil.DamageGaugeColor(gaugeColor));
        lifeGauge.material.SetColor("_BackGroundColor",
            KX_netUtil.GaugeBlankColor(gaugeColor));
        lifeGauge.material.SetColor("_EdgeColor",
            KX_netUtil.GaugeBlankColor(
                lifeGauge.material.GetColor("_BackGroundColor")
            ));

        lifeGauge.material.SetFloat("_FillAmount1", liveEntity.GetHPAmount());

        if (lifeGauge.material.GetFloat("_FillAmount1")
                         <= lifeGauge.material.GetFloat("_FillAmount2"))
        {
            lifeGauge.material.SetFloat("_FillAmount2",
                lifeGauge.material.GetFloat("_FillAmount2") - 0.005f);
        }
        else
        {
            lifeGauge.material.SetFloat("_FillAmount2",
                lifeGauge.material.GetFloat("_FillAmount1"));
        }
        //キャラ名表示を更新
        name.text = liveEntity.GetData().GetCharaName();
    }

    void OnWillRenderObject()
    {
        
    }
}
