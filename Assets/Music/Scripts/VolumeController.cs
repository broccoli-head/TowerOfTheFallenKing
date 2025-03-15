using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        //³aduje volume
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        volumeSlider.value = savedVolume;
        setVolume(savedVolume);
        volumeSlider.onValueChanged.AddListener(setVolume);
    }

    private void setVolume(float volume)
    {
        //zapisuje zeby bylo nawet po wylaczeniu gry
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}
