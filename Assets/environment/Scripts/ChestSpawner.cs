using System.Collections.Generic;
using UnityEngine;


public class ChestSpawner : MonoBehaviour
{
    public bool spawnAfterClearing = false;
    public AudioClip appearSound;

    private Collider2D collider;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool spawned = false;
    public static List<string> claimedChests = new List<string>();

    void Start()
    {
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (spawnAfterClearing)
        {
            collider.enabled = false;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);

            foreach(Transform child in transform)
            {
                child.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            }
        }
        else spawned = true;
    }

    void Update()
    {
        if (!spawned && !claimedChests.Contains(Level.CurrentlyOnRoom))
        {
            if (LevelLoader.CleanedRooms.Contains(Level.CurrentlyOnRoom))
            {
                if(!audioSource.isPlaying)
                    audioSource.PlayOneShot(appearSound);
                
                collider.enabled = true;
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
                spawned = true;

                foreach (Transform child in transform)
                {
                    child.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
    }
}
