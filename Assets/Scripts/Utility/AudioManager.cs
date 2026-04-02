using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip TestSFXAudioClip;
    [SerializeField] private AudioClip HoverSound;
    [SerializeField] private AudioClip ClickSound;
    [SerializeField] private GameObject pfaudio;

    // Keys in the AudioMixer
    [SerializeField] private const string MusicVolumeParam = "BGM";
    [SerializeField] private const string SFXVolumeParam = "SFX";
    [SerializeField] private GameObject BGMSlider;
    [SerializeField] private GameObject SFXSlider;

    public static AudioManager instance;
    [SerializeField] private AudioClip LoseAudioClip;

    /// Set music volume using slider value (0 to 1)

    private void Start()
    {
        ScriptManager.MakeManager(this, ref instance);
    }
    public void SetMusicVolume()
    {
        // Convert 0-1 slider to -80 to 0 dB
        float value = BGMSlider.GetComponent<Slider>().value;
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(MusicVolumeParam, dB);
    }


    /// Set sound effects volume using slider value (0 to 1)

    public void SetSFXVolume()
    {
        float value = SFXSlider.GetComponent<Slider>().value;
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(SFXVolumeParam, dB);
        PlaySFX(TestSFXAudioClip);
    }

    public void PlaySFX(AudioClip audioClip)
    {
        GameObject audioObj = Instantiate(pfaudio);
        AudioSource source = audioObj.GetComponent<AudioSource>();
        source.spatialBlend = 0f;

        source.PlayOneShot(audioClip);
        Destroy(audioObj, audioClip.length);
    }
    public void PlaySFX(AudioClip audioClip,Transform audioPosition)
    {
        GameObject audioObj = Instantiate(pfaudio,audioPosition);
        AudioSource source = audioObj.GetComponent<AudioSource>();
        source.spatialBlend = 1f;

        source.PlayOneShot(audioClip);
        Destroy(audioObj, audioClip.length);
    }

    public void hoverSound()
    { 
        PlaySFX(HoverSound);
    }
    public void ClickedSound()
    { 
        PlaySFX(ClickSound);
    }

    public void LoseAudio()
    {
        PlaySFX(LoseAudioClip);
    }
}
