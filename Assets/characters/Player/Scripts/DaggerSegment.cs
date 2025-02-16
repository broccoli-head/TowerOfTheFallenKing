using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DaggerSegment : MonoBehaviour
{
    PlayerDagger dagger;

    private void Start()
    {
        dagger = FindAnyObjectByType<PlayerDagger>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        dagger.TriggerEnter(other);
    }
}
