using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource PlaySound;

    public AudioClip MenuMusic;
    public AudioClip Level1;
    public AudioClip Boss1;
    public bool destroy = false;


    // Start is called before the first frame update
    void Start()
    {
        PlaySound.clip = Level1;
        PlaySound.Play();
    }

    void Awake()
    {
        //w menu glownym chcemy aby usuwalo, zas w ciagu gry juz nie 
        if (destroy)
            DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        int level = SceneManager.GetActiveScene().buildIndex;

    }
}
