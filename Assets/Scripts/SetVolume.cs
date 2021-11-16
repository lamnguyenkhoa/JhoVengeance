using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    public AudioMixer bgmMixer;
    public Slider bgmVolumeSlider;
    public AudioMixer sfxMixer;
    public Slider sfxVolumeSlider;

    private void OnEnable()
    {
        float tmp;

        bgmMixer.GetFloat("OverallVolume", out tmp);
        tmp = tmp / 20;
        tmp = Mathf.Pow(10, tmp);
        bgmVolumeSlider.value = tmp;

        sfxMixer.GetFloat("OverallVolume", out tmp);
        tmp = tmp / 20;
        tmp = Mathf.Pow(10, tmp);
        sfxVolumeSlider.value = tmp;
    }

    public void SetBGMLevel(float sliderValue)
    {
        bgmMixer.SetFloat("OverallVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSFXLevel(float sliderValue)
    {
        sfxMixer.SetFloat("OverallVolume", Mathf.Log10(sliderValue) * 20);
    }
}