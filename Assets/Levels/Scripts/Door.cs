using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum Type
    {
        NextLevel,
        PrevLevel,
        SideLevel,
        MainLevel
    }

    public Type type;
    [Min(0)] public int index;
    public bool KillAllEnemies;
    public string[] RequiredItems = { };

    List<Enemy> Enemies;
    Inventory inventory;
    LevelLoader levelLoader;
    bool AllEnemiesKilled = false;
    bool AllItemsColected = false;
    bool NoPrevLevel = false;
    string DestinationName = "Unset";


    private void Awake()
    {
        levelLoader = LevelLoader.Instance;

        // pobiera nazwe pokoju do ktorego prowadza drzwi
        GetDestinationName();

        // jesli nazwa pokoju z ktorego przyszedl gracz to nazwa pokoju do ktorego prowadza drzwi
        // lub nasz obiekt prowadzi do poprzedniego levelu ktory nie istnieje, spawnuje obiekt gracza
        if (Level.ComingFromRoom == DestinationName || (type == Type.PrevLevel && NoPrevLevel))
        {
            var Player = Instantiate(levelLoader.PlayerPrefab, transform.position, Quaternion.identity);
            
            // jesli nie ma poprzedniego levelu wylacza ladowanie Hp z pliku, w przeciwnym wypadku je wlacza
            Player.GetComponent<PlayerLive>().load = !NoPrevLevel;
        }
    }


    private void Start()
    {
        inventory = Inventory.Instance;
        


        if (KillAllEnemies)
        {
            Enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {

            AllEnemiesKilled = true;
            if (KillAllEnemies)
            { 
                foreach (Enemy enemy in Enemies)
                {
                    try
                    {
                        if (enemy.HP > 0)
                            AllEnemiesKilled = false;
                            break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            levelLoader.ActualLevel.Cleaned = AllEnemiesKilled;


            AllItemsColected = true;
            foreach (string item in RequiredItems) 
            {
                Item i = inventory.FindItemByName(item);
                if (!inventory.PlayerHaveItem(i).Item1)
                {
                    AllItemsColected = false;
                    break;
                }
            }

            if (AllEnemiesKilled && AllItemsColected)
            {
                if (type == Type.NextLevel)
                    levelLoader.NextLevel(index);

                else if (type == Type.SideLevel)
                    levelLoader.SideLevel(index);

                else if (type == Type.PrevLevel)
                    levelLoader.PrevLevel();

                else if (type == Type.MainLevel)
                    levelLoader.MainLevel();
            }
        }
    }


    private void GetDestinationName()
    {
        if (type == Type.NextLevel)
        {
            if (levelLoader.ActualLevel.NextLevels.Length > index)
            {
                DestinationName = levelLoader.ActualLevel.NextLevels[index].LevelName;
            }
        }
        else if (type == Type.PrevLevel)
        {
            if (levelLoader.ActualLevel.PreviousLevel != null)
                DestinationName = levelLoader.ActualLevel.PreviousLevel.LevelName;
            else
            {
                NoPrevLevel = true;
            }
                
        }
        else if (type == Type.SideLevel)
        {
            if (levelLoader.ActualLevel.SideLevels.Length > index)
            {
                DestinationName = levelLoader.ActualLevel.SideLevels[index];
            }
        }
        else if (type == Type.MainLevel)
        {
            DestinationName = levelLoader.ActualLevel.LevelName;
        }
    }
}
