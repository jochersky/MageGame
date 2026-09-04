using UnityEngine;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolParamName = "MasterVolume";
    [SerializeField] private string SFXVolParamName = "SFXVolume";
    [SerializeField] private string musicVolParamName = "MusicVolume";
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(masterVolParamName, Mathf.Log10(volume) * 20f);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat(SFXVolParamName, Mathf.Log10(volume) * 20f);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(musicVolParamName, Mathf.Log10(volume) * 20f);
    }
}
