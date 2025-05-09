using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanedCheck : MonoBehaviour
{
    public List<Enemy> Enemies;
    public List<Spawner> Spawners;
    bool AllEnemiesKilled = false;

    void Start()
    {
        Enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        Spawners = new List<Spawner>(FindObjectsOfType<Spawner>());
        if (LevelLoader.CleanedRooms.Contains(Level.CurrentlyOnRoom))
        {
            Debug.Log(Level.CurrentlyOnRoom + " is already cleaned");
            Destroy(gameObject);
        }
        
    }

    void Update()
    {
        AllEnemiesKilled = true;
        Enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        Spawners = new List<Spawner>(FindObjectsOfType<Spawner>());
        foreach (Enemy enemy in Enemies)
        {
            if (enemy != null)
            {
                if (enemy.HP > 0)
                {
                    AllEnemiesKilled = false;
                    break;
                }
            }
        }
        foreach(Spawner spawner in Spawners)
        {
            if (spawner != null)
            {
                AllEnemiesKilled = false;
                break;
            }
                
        }
        if (AllEnemiesKilled)
        {
            if (!LevelLoader.CleanedRooms.Contains(Level.CurrentlyOnRoom))
            {
                Debug.Log("Wyczyszczono " + Level.CurrentlyOnRoom);
                LevelLoader.CleanedRooms.Add(Level.CurrentlyOnRoom);
                Destroy(this);
            }
        }
    }
}
