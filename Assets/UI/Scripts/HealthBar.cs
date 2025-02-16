using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HealthBar : MonoBehaviour
{
    PlayerLive playerLive;
    Image img;
    
    void Start()
    {
        img = GetComponent<Image>();
        img.type = Image.Type.Filled;
        playerLive = FindAnyObjectByType<PlayerLive>();
    }

    
    void Update()
    {
        img.fillAmount = (playerLive.HitPoints / playerLive.StartHP);
    }
}
