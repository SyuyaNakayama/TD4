using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LiveEntityHUD : MonoBehaviour
{
    const float batteryGaugeShiftIntensity = 0.4f;

    [SerializeField]
    LiveEntity liveEntity;
    [SerializeField]
    SpriteRenderer lifeGauge;
    [SerializeField]
    SpriteRenderer batteryGauge;
    [SerializeField]
    Vector3 batteryGaugePos;
    [SerializeField]
    Vector3 hideBatteryGaugePos;
    [SerializeField]
    TMP_Text name;

    void FixedUpdate()
    {
        //�̗̓Q�[�W���X�V
        Color lifeGaugeColor = liveEntity.GetCassetteData().GetThemeColor();
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

        lifeGauge.material.SetFloat("_FillAmount1", liveEntity.GetLife());

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
        //�d�̓Q�[�W��K�؂Ȉʒu��
        Vector3 targetPos = batteryGaugePos;
        if (liveEntity.GetBatteryAmount() >= 1)
        {
            targetPos = hideBatteryGaugePos;
        }
        batteryGauge.transform.localPosition = Vector3.Lerp(
            batteryGauge.transform.localPosition, targetPos,
            batteryGaugeShiftIntensity);

        //�L�������\�����X�V
        name.text = liveEntity.GetCassetteData().GetCharaName();
    }

    void OnWillRenderObject()
    {

    }
}