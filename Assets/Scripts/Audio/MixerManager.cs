using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider masterSlider;
    [SerializeField] private string masterVolParamName = "MasterVolume";
    [SerializeField] private string SFXVolParamName = "SFXVolume";
    [SerializeField] private string musicVolParamName = "MusicVolume";
    readonly float DECIBEL_CONSTANT = 20f;
    void Start()
    {
        SyncSliders();
    }

    void SyncSliders()
    {
        audioMixer.GetFloat(masterVolParamName, out float volume);
        masterSlider.value = (float)Math.Pow(10.0, volume / DECIBEL_CONSTANT);
        audioMixer.GetFloat(musicVolParamName, out volume);
        musicSlider.value = (float)Math.Pow(10.0, volume / DECIBEL_CONSTANT);
        audioMixer.GetFloat(SFXVolParamName, out volume);
        sfxSlider.value = (float)Math.Pow(10.0, volume / DECIBEL_CONSTANT);
    }
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(masterVolParamName, Mathf.Log10(volume) * DECIBEL_CONSTANT);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat(SFXVolParamName, Mathf.Log10(volume) * DECIBEL_CONSTANT);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(musicVolParamName, Mathf.Log10(volume) * DECIBEL_CONSTANT);
    }
}
