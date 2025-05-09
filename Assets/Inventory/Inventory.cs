using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class Inventory : MonoBehaviour, Saveable 
{
    
    public TMP_FontAsset font;

    [Header("Dostepne w grze")]
    public Potion[] potions;
    public Resource[] resources;
    public List<Item> OtherItems;
    public Recipe[] recipes;
    public Interaction[] interactions;


    [Header("Inventory gracza")]
    public List<PlayerItem> PlayerItems;


    [HideInInspector] public Potion[] QuickPotions = new Potion[3];
    [HideInInspector] public Potion SelectedPotion;
    [HideInInspector] public Item SelectedItem;
    [HideInInspector] public Item PointedItem;

    private int index = 1;


    public static Inventory Instance { get; private set; }
    public static bool LoadFromFile;


    private void Awake()
    {
        // jesli na scenie jest juz inventory usuwamy nasz skrypt
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // jesli nie ma innego obiektu ustawiamy swoj obiekt jako glowny
        else
            Instance = this;

        
        // ladujemy inventory z pliku
        if (LoadFromFile)
            Load();

    }

    private void Start()
    {
        if (!LoadFromFile)
        {
            // jeœli nie ³adowaliœmy inventory z pliku, zrobimy to w nastêpnym pokoju
            LoadFromFile = true;

            // wype³niamy quick potions
            int j = 0;
            for (int i = 0; i < QuickPotions.Length; i++)
            {
                try
                {
                    Potion potion = null;
                    while (potion == null)
                    {
                        if (j >= PlayerItems.Count)
                            break;

                        potion = FindPotionByName(PlayerItems[j].Name);

                        j++;
                    }

                    QuickPotions[i] = potion;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    break;
                }

            }
            
            SelectedPotion = QuickPotions[index];
        }
    }

    private void Update()
    {
        ValidatePotions();
        //przelaczanie pomiedzy QuickPotions
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
            try
            {
                float dpi = Screen.dpi;
                if (dpi == 0) dpi = 96f;    //jezeli nie mozna odczytac dpi ustawiamy 96
                float screenScale = dpi / 96f;
                int cursorSize = Mathf.RoundToInt(64 * screenScale);  //wielkosc kursora zalezna od dpi ekranu

                Texture2D cursorTexture = Helper.TextureFromSprite(SelectedItem.GetSprite());
                cursorTexture = Helper.ResizeTexture(cursorTexture, cursorSize, cursorSize);

                Vector2 cursorHotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
                Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.ForceSoftware);
            }
            catch
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            
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


    public void RemovePlayerItem(string name)
    {
        for (int i = 0; i < PlayerItems.Count; i++)
        {
            if (PlayerItems[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                PlayerItems[i].count--;
                if(PlayerItems[i].count <= 0) 
                    PlayerItems.RemoveAt(i);
                break;
            }
        }
    }

    public PlayerItem FindPlayerItemByName(string name)
    {
        for (int i = 0; i < PlayerItems.Count; i++)
        {
            if (PlayerItems[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return PlayerItems[i];
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


    //zwraca item, jesli nie ma itemu o takiej nazwie zwraca null
    public Item FindItemByName(string name)
    {
        Item item = FindPotionByName(name);

        if (item == null)
            item = FindResourceByName(name);

        if (item == null)
        {
            foreach (var i in OtherItems)
            {
                if (i.Name == name)
                {
                    item = i;
                    break;
                }
            }       
        }

        return item;
    }

    public void AddPlayerItem(string name)
    {
        AddPlayerItem(name, 1);
    }
  
    public void AddPlayerItem(string name, int count)
    {
        bool Alreadyexist = false;
        foreach(var item in PlayerItems)
        {
            if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                item.count += count;
                Alreadyexist = true;
            }      
        }

        if(!Alreadyexist)
        {
            PlayerItems.Add(new PlayerItem(name, count));
        }
    }

    //zwraca czy gracz posiada dany item i jesli tak, ilosc posiadanych itemow
    public (bool, int) PlayerHaveItem(Item item)
    {
        if(item == null)
            return (false, 0);

        foreach (var i in PlayerItems) {
            if (i.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                return(true, i.count);
        }

        return (false, 0);
    }

    public void AddPlayerItem(Item item, int count = 1)
    {
        if (item != null && count > 0)
        {
            AddPlayerItem(item.Name, count);
        }
        else
            Debug.Log("Do funkcji Inventory::AddPlayerItem() przekazano nieprawidlowe wartosci: Item: " + item + ", Count: " + count); 
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
            PlayerItems = this.PlayerItems,
            QuickPotions = this.QuickPotions,
            SelectedPotion = this.SelectedPotion,
            index = this.index
        };

        string json = JsonUtility.ToJson(invData);

        string filePath = Application.persistentDataPath + "/Inventory.json";

        Debug.Log("Inventory saved at " + filePath);

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
            PlayerItems = new List<PlayerItem>(loadedInventory.PlayerItems);
            QuickPotions = loadedInventory.QuickPotions;
            SelectedPotion = loadedInventory.SelectedPotion;
            SelectedItem = loadedInventory.SelectedItem;
            PointedItem = loadedInventory.PointedItem;
            index = loadedInventory.index;

            Debug.Log("Inventory zaladowano z pliku.");
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
        public List<PlayerItem> PlayerItems;
        public Potion[] QuickPotions = new Potion[3];
        public Potion SelectedPotion;
        public Item SelectedItem;
        public Item PointedItem;
        public int index = 1;
    }

}


[System.Serializable]
public class PlayerItem
{
    public string Name;
    public int count;

    public PlayerItem(string name, int count ) {
        Name = name;
        this.count = count;
    }
}

