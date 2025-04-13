using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MusicVolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        //³aduje zapisan¹ glosnosc
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        setMusicVolume(musicVolume);
        setSFXVolume(sfxVolume);

        musicSlider.onValueChanged.AddListener(setMusicVolume);
        sfxSlider.onValueChanged.AddListener(setSFXVolume);
    }

    private void setMusicVolume(float volume)
    {
        //zmienia glosnosc na decybele
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(volume, 0.001f)) * 20);

        //zapisuje zeby bylo nawet po wylaczeniu gry
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    private void setSFXVolume(float volume)
    {
        //zmienia glosnosc na decybele
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(volume, 0.001f)) * 20);

        //zapisuje zeby bylo nawet po wylaczeniu gry
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void OnDisable()
    {
        //po ukryciu okna ustawien, zapisuje glosnosc
        PlayerPrefs.Save();
    }
}
