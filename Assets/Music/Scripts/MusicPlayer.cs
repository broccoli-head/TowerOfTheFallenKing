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

        int randomIndex = Random.Range(0, soundTracks.Length);
        PlaySound.clip = soundTracks[randomIndex];
        PlaySound.Play();
    }
}
