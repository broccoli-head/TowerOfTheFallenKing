using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanedCheck : MonoBehaviour
{
    List<Enemy> Enemies;
    bool AllEnemiesKilled = false;

    void Start()
    {
        Enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        if (LevelLoader.CleanedRooms.Contains(Level.CurrentlyOnRoom))
        {
            Debug.Log(Level.CurrentlyOnRoom + " is already cleaned");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        AllEnemiesKilled = true;
        foreach (Enemy enemy in Enemies)
        {
            try
            {
                if (enemy?.HP > 0)
                {
                    AllEnemiesKilled = false;
                    break;
                }
            }
            catch
            {
                continue;
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
