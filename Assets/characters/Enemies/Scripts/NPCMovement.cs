using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Enemy))]
public class NPCMovement : MonoBehaviour, Controller
{
    Move[] moves;
    Move move;
    Rigidbody2D rb;
    Enemy enemy;

    float speed;
    bool looping;
    int index = 0;
    float time = 0;

    AudioSource audioSource;
    AudioClip footstepsSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();

        moves = enemy.moves;
        speed = enemy.speed;
        looping = enemy.looping;

        audioSource = GetComponent<AudioSource>();
        footstepsSound = enemy.footstepsSound;

        try
        {
            move = moves[index];
        }
        catch (IndexOutOfRangeException e)
        {
            Destroy(this);
        }
    }

    void Update()
    {
        speed = enemy.speed;

        if (audioSource != null)
        {
            //puœæ dŸwiêk chodzenia, je¿eli przeciwnik siê porusza
            if (rb.velocity.magnitude > 0.1f && CommlinkOpener.checkVisibility())
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = footstepsSound;
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }
        }
    }


    void FixedUpdate()
    {
        if (enemy.FreezeTime <= 0)
        {
            try
            {
                rb.velocity = move.direction.normalized * speed;
            }

            catch (Exception e) {
                Destroy(this);
                return;
            }

            time += Time.fixedDeltaTime;
            if(time >= move.time)
            {
                time = 0;
                index++;
                if(index >= moves.Length)
                {
                    if (looping)
                        index = 0;
                    else
                    {
                        index--;
                        speed = 0;
                    }
                        
                }
                move = moves[index];
            }
            //kierunek w ktorym patrzy przeciwnik, uzywane w Detection
            enemy.FacingDirection = move.direction.normalized;
        }
        else
        {
            //kierunek w ktorym patrzy przeciwnik, uzywane w Detection
            enemy.FacingDirection = Vector2.down;

            enemy.FreezeTime -= Time.fixedDeltaTime;
            if (enemy.FreezeTime < 0)
                enemy.FreezeTime = 0;
        }

    }
    public void Disable(float time)
    {
        enemy.FreezeTime += time;
    }

}

[System.Serializable]
public class Move
{
    public Vector2 direction;
    [Min(0)]public float time;
}