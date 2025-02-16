using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Inventory : MonoBehaviour, Saveable 
{
    public bool LoadFromFile;

    [Header("Dostêpne w grze")]
    public Potion[] potions;
    public Resource[] resources;
    public Recipe[] recipes;
    public Interaction[] interactions;


    [Header("Inventory gracza")]
    public List<PlayerPotion> PlayerPotions;
    public List<PlayerResource> PlayerResources = new List<PlayerResource>();


    [HideInInspector] public Potion[] QuickPotions = new Potion[3];
    [HideInInspector] public Potion SelectedPotion;
    [HideInInspector] public Item SelectedItem;
    [HideInInspector] public Item PointedItem;
    private int index = 1;

    public static Inventory Instance { get; private set; }

    private void Awake()
    {
        // jeœli na scenie jest ju¿ inventory usuwamy nasz skrypt
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // jesli nie ma innego obiektu ustawiamy swoj obiekt jako glowny
        else
            Instance = this;

        if (LoadFromFile)
            Load();
    }

    private void Start()
    {
        for (int i = 0;i < QuickPotions.Length;i++)
        {
            try
            {
                QuickPotions[i] = FindPotionByName(PlayerPotions[i].PotionName);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                break;
            }
            
        }
        SelectedPotion = QuickPotions[index];
    }

    private void Update()
    {
        ValidatePotions();
        //prze³¹czanie pomiêdzy QuickPotions
        if(Input.GetKeyDown(KeyCode.E))
            index++;
        else if(Input.GetKeyDown(KeyCode.Q))
            index--;
        if(index < 0) 
            index = QuickPotions.Length - 1;
        if (index >= QuickPotions.Length)
            index = 0;
        SelectedPotion = QuickPotions[index];


        //zmiana kursora
        if(SelectedItem == null)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        else
        {
            Texture2D cursorTexture = ComponentHelper.TextureFromSprite( SelectedItem.GetSprite() );
            Vector2 cursorHotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
            Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.ForceSoftware);
        }
        
    }

    public void RefreshPlayerInventory()
    {
        foreach (var item in GameObject.FindObjectsOfType<ItemsUI>() )
        {
            item.RefreshItemsList();
        }
    }

    public void ValidatePotions()
    {
        for (int i = 0; i < QuickPotions.Length; i++)
        {
            if (QuickPotions[i] == null)
                continue;
            if (!PlayerHaveItem(QuickPotions[i]).Item1)
                QuickPotions[i] = null;
        }
        SelectedPotion = QuickPotions[index];
    }
        
    public Potion FindPotionByName(string name)
    {
        for (int i = 0; i < potions.Length; i++)
        {
            if (potions[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return potions[i];
        }
        return null;
    }

    public void AddPlayerPotion(string name)
    {
        AddPlayerPotion(name, 1);
    }
  
    public void AddPlayerPotion(string name, int count)
    {
        bool PotionWasInPlayerPotions = false;
        for (int i = 0; i < PlayerPotions.Count; i++)
        {
            if (PlayerPotions[i].PotionName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                PlayerPotions[i].count+= count;
                PotionWasInPlayerPotions = true;
                break;
            }
        }
        if (!PotionWasInPlayerPotions)
            PlayerPotions.Add(new PlayerPotion(name,count));
    }


    public void RemovePlayerPotion(string name)
    {
        for (int i = 0; i < PlayerPotions.Count; i++)
        {
            if (PlayerPotions[i].PotionName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                PlayerPotions[i].count--;
                if(PlayerPotions[i].count <= 0) 
                    PlayerPotions.RemoveAt(i);
                break;
            }
        }
    }

    public PlayerPotion FindPlayerPotionByName(string name)
    {
        for (int i = 0; i < PlayerPotions.Count; i++)
        {
            if (PlayerPotions[i].PotionName.Equals(name, StringComparison.OrdinalIgnoreCase))
                return PlayerPotions[i];
        }
        return null;
    }


    public Resource FindResourceByName(string name)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (resources[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return resources[i];
        }
        return null;
    }


    public void AddPlayerResource(string name)
    {
        AddPlayerResource(name, 1);
    }
    public void AddPlayerResource(string name, int count)
    {
        bool ResourceWasInPlayerResources = false;
        for (int i = 0; i < PlayerPotions.Count; i++)
        {
            if (PlayerResources[i].ResourceName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                PlayerResources[i].count += count;
                ResourceWasInPlayerResources = true;
                break;
            }
        }
        if (!ResourceWasInPlayerResources)
            PlayerResources.Add(new PlayerResource(name, count));
    }


    public void RemovePlayerResource(string name)
    {
        for (int i = 0; i < PlayerResources.Count; i++)
        {
            if (PlayerResources[i].ResourceName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                PlayerResources[i].count--;
                if (PlayerResources[i].count <= 0)
                    PlayerResources.RemoveAt(i);
                break;
            }
        }
    }

    public PlayerResource FindPlayerResourceByName(string name)
    {
        for (int i = 0; i < PlayerResources.Count; i++)
        {
            if (PlayerResources[i].ResourceName.Equals(name, StringComparison.OrdinalIgnoreCase))
                return PlayerResources[i];
        }
        return null;
    }


    //zwraca potion lub resource, jesli nie ma ani mikstury ani zasobu o takiej nazwie zwraca null
    public Item FindItemByName(string name)
    {
        Item item = FindPotionByName(name);
        if (item == null)
        {
            item = FindResourceByName(name);
            return item;
        }
        else return item;
    }


    //zwraca czy gracz posiada dany item i jeœli tak, ilosc posiadanych itemow
    public (bool, int) PlayerHaveItem(Item item)
    {
        if(item == null)
            return (false, 0);

        int count;
        if (item.IsPotion())
        {
            count = FindPlayerPotionByName(item.GetName()) != null ? FindPlayerPotionByName(item.GetName()).count : 0;
            if(count <= 0)
                return (false, 0);
            else
                return (true, count);
        }
        else
        {
            count = FindPlayerResourceByName(item.GetName()) != null ? FindPlayerResourceByName(item.GetName()).count : 0;
            if (count <= 0)
                return (false, 0);
            else
                return (true, count);
        }
    }

    public void AddPlayerItem(string name,int count = 1)
    {
        AddPlayerItem(FindItemByName(name), count);
    }

    public void AddPlayerItem(Item item, int count = 1)
    {
        if (item != null && count > 0)
        {
            if (item.IsPotion())
                AddPlayerPotion(item.GetName(), count);
            else
                AddPlayerResource(item.GetName(), count);
        }
        else
            Debug.Log("Do funkcji Inventory::AddPlayerItem() przekazano nieprawid³owe wartoœci: Item: " + item + ", Count: " + count); 
    }

    public Component Interact(string ActivePotion, string OtherPotion)
    {
        foreach (var interaction in interactions)
        {
            if (interaction.PotionName.Equals(ActivePotion, StringComparison.OrdinalIgnoreCase))
            {
                return interaction.find(OtherPotion);
            }
        }
        return null;
    }


    public void Save()
    {
        Data invData = new Data()
        {
            potions = this.potions,
            resources = this.resources,
            recipes = this.recipes,
            interactions = this.interactions,
            PlayerPotions = this.PlayerPotions,
            PlayerResources = this.PlayerResources,
            QuickPotions = this.QuickPotions,
            SelectedPotion = this.SelectedPotion,
        };

        string json = JsonUtility.ToJson(invData);

        string filePath = Application.persistentDataPath + "/Inventory.json";

        Debug.Log(filePath);

        File.WriteAllText(filePath, json);
    }

    public void Load()
    {
        string filePath = Application.persistentDataPath + "/Inventory.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Data loadedInventory = JsonUtility.FromJson<Data>(json);

            potions = loadedInventory.potions;
            resources = loadedInventory.resources;
            recipes = loadedInventory.recipes;
            interactions = loadedInventory.interactions;
            PlayerPotions = new List<PlayerPotion>(loadedInventory.PlayerPotions);
            PlayerResources = new List<PlayerResource>(loadedInventory.PlayerResources);
            QuickPotions = loadedInventory.QuickPotions;
            SelectedPotion = loadedInventory.SelectedPotion;
            SelectedItem = loadedInventory.SelectedItem;
            PointedItem = loadedInventory.PointedItem;

            Debug.Log("Inventory za³adowano z pliku.");
        }
        else
        {
            Debug.LogWarning("Plik Inventory.json nie istnieje.");
        }
    
    }

    private class Data
    {

        public Potion[] potions;
        public Resource[] resources;
        public Recipe[] recipes;
        public Interaction[] interactions;
        public bool LoadFromFile;
        public List<PlayerPotion> PlayerPotions;
        public List<PlayerResource> PlayerResources = new List<PlayerResource>();
        public Potion[] QuickPotions = new Potion[3];
        public Potion SelectedPotion;
        public Item SelectedItem;
        public Item PointedItem;
        public int index = 1;
    }

}


[System.Serializable]
public class PlayerPotion
{
    public string PotionName;
    public int count;

    public PlayerPotion(string name, int count ) {
        PotionName = name;
        this.count = count;
    }
}


[System.Serializable]
public class PlayerResource
{
    public string ResourceName;
    public int count;

    public PlayerResource(string name, int count ) {
        ResourceName = name;
        this.count = count;
    }
}