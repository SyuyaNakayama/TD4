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
    SpriteRenderer batteryGauge;
    [SerializeField]
    TMP_Text name;

    void FixedUpdate()
    {
        //�̗̓Q�[�W���X�V
        Color lifeGaugeColor = liveEntity.GetData().GetThemeColor();
        lifeGaugeColor.a = 1;
        lifeGauge.material.SetColor("_GaugeColor1", lifeGaugeColor);
        lifeGauge.material.SetColor("_GaugeColor2",
            KX_netUtil.DamageGaugeColor(lifeGaugeColor));
        lifeGauge.material.SetColor("_BackGroundColor",
            KX_netUtil.GaugeBlankColor(lifeGaugeColor));
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
        //�d�̓Q�[�W���X�V
        Color batteryGaugeColor = new Color(0, 0.8f, 0, 1);
        if (liveEntity.IsShield())
        {
            batteryGaugeColor = new Color(1, 1, 0, 1);
        }
        else if (!liveEntity.GetShieldable())
        {
            batteryGaugeColor = new Color(1, 0, 0, 1);
        }
        batteryGauge.material.SetColor("_GaugeColor1", batteryGaugeColor);
        batteryGauge.material.SetColor("_BackGroundColor",
            KX_netUtil.GaugeBlankColor(batteryGaugeColor));
        batteryGauge.material.SetColor("_EdgeColor",
            KX_netUtil.GaugeBlankColor(
                batteryGauge.material.GetColor("_BackGroundColor")
            ));
        batteryGauge.material.SetFloat("_FillAmount1", liveEntity.GetBatteryAmount());
        batteryGauge.material.SetFloat("_FillAmount2", 0);

        //�L�������\�����X�V
        name.text = liveEntity.GetData().GetCharaName();
    }

    void OnWillRenderObject()
    {

    }
}