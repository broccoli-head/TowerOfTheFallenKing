using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance;
    public AudioSource PlaySound;
    public AudioClip[] soundTracks;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        PlaySound.clip = soundTracks[0];
        PlaySound.Play();
    }

    void Update()
    {
        if (Level.CurrentlyOnRoom == "Mushroom Boss Fight")
        {
            PlaySound.clip = soundTracks[1];
            if (!PlaySound.isPlaying)
            {
                PlaySound.Play();
            }
        }

        else
        {
            PlaySound.clip = soundTracks[0];
            if (!PlaySound.isPlaying)
            {
                PlaySound.Play();
            }
        }
    }
}
