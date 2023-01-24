using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class Audio : MonoBehaviour
{
    public Slider audioSlider;
    public AudioMixer masterMixer;
    public Image audioBtnImg;
    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
        audioBtnImg.color = audioBtnImg.color == Color.black ? Color.white : Color.black;
    }

    public void AudioControl()
    {
        float sound = audioSlider.value;
        if (sound == -40f)
            masterMixer.SetFloat("BGM", -80);
        else
            masterMixer.SetFloat("BGM", sound);
    }
}
