using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class TeleportationMovement : MonoBehaviour
{

    TeleportationPoint[] TeleportationPoints;
    TeleportationPoint CurrentPoint;
    System.Random rand;
    int TepCount;
    PlayerLive player;
    Enemy enemy;

    public float RunAwayDistance;

    private void Awake()
    {
        var tp = Instantiate(new GameObject(), transform.position, Quaternion.identity);
        tp.AddComponent<TeleportationPoint>();
    }

    void Start()
    {
        TeleportationPoints = FindObjectsOfType<TeleportationPoint>();
        rand = new System.Random();
        player = FindFirstObjectByType<PlayerLive>();
        enemy = GetComponent<Enemy>();

    }

    private void Update()
    {
        float distance = Vector2.Distance(player.transform.position, transform.position);
        if(enemy.PlayerDetected && distance <= RunAwayDistance)
        {
            Teleport();
        }

        // Jezeli przeciwnik jest "zamrozony" zmniejsza czas pozostaly do odmrozenia i przerywa wykonywanie funkcji
        if (enemy.FreezeTime > 0)
        {
            enemy.FreezeTime -= Time.fixedDeltaTime;
            if (enemy.FreezeTime < 0)
                enemy.FreezeTime = 0;
        }


    }

    public void Teleport()
    {
        if (enemy.FreezeTime > 0)
            return;

        int index = rand.Next(0,TeleportationPoints.Length);
        if (TeleportationPoints[index].Taken || TeleportationPoints[index] == CurrentPoint)
        {
            if (TepCount < 10) 
            {
                TepCount++;
                Teleport();
            }
            else
                return;
        }
        TepCount = 0;
        if(CurrentPoint != null) 
            CurrentPoint.Taken = false;
        CurrentPoint = TeleportationPoints[index];
        CurrentPoint.Taken = true;
        transform.position = CurrentPoint.transform.position;
    }
}
