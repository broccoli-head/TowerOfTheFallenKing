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

    Inventory inventory;
    LevelLoader levelLoader;
    bool AllItemsColected = false;
    bool NoPrevLevel = false;
    bool Active = false;
    string DestinationName = "Unset";


    private void Awake()
    {
        levelLoader = LevelLoader.Instance;

        // pobiera nazwe pokoju do ktorego prowadza drzwi
        GetDestinationName();

        // jesli nazwa pokoju z ktorego przyszedl gracz to nazwa pokoju do ktorego prowadza drzwi
        // lub nasz obiekt prowadzi do poprzedniego levelu ktory nie istnieje, spawnuje obiekt gracza
        if (Level.ComingFromRoom == DestinationName || (type == Type.PrevLevel && NoPrevLevel && Level.ComingFromRoom == "Default"))
        {
            var Player = Instantiate(levelLoader.PlayerPrefab, transform.position, Quaternion.identity);

            // jesli nie ma poprzedniego levelu wylacza ladowanie Hp z pliku, w przeciwnym wypadku je wlacza
            Player.GetComponent<PlayerLive>().load = !NoPrevLevel;
        }


        var rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        

        if (gameObject.TryGetComponent<Collider2D>(out var T))
            T.isTrigger = true;
        else
            gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
    }


    private void Start()
    {
        inventory = Inventory.Instance;

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
            Active = true;
        else
            Active = false;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Active && collision.gameObject.CompareTag("Player"))
        { 

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

            if (
                (LevelLoader.CleanedRooms.Contains(Level.CurrentlyOnRoom) || !KillAllEnemies)
                && AllItemsColected
            ){
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
            {
                DestinationName = levelLoader.ActualLevel.PreviousLevel.LevelName;
            }
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
