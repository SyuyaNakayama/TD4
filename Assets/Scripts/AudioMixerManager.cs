using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [Serializable]
    struct ChannelAndSlider
    {
        public string channel;
        public CMBSlider slider;
    }

    [SerializeField]
    AudioMixer audioMixer;
    [SerializeField]
    ControlMapMenu controlMap;
    [SerializeField]
    ChannelAndSlider[] channelAndSliders;

    void FixedUpdate()
    {
        if (controlMap.IsUserControl())
        {
            for (int i = 0; i < channelAndSliders.Length; i++)
            {
                audioMixer.SetFloat(
                    channelAndSliders[i].channel,
                    channelAndSliders[i].slider.GetScaledOutputValue());
            }
        }
    }
}