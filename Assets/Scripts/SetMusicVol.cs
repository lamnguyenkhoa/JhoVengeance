using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetMusicVol : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider volumeSlider;

    private void OnEnable()
    {
        float tmp;
        mixer.GetFloat("BackgroundMusic", out tmp);
        tmp = tmp / 20;
        tmp = Mathf.Pow(10, tmp);
        volumeSlider.value = tmp;
    }

    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("BackgroundMusic", Mathf.Log10(sliderValue) * 20);
    }
}