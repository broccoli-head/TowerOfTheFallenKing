using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    Inventory inventory;
    CraftingSlot[] slots;
    Item result;
    int ResultCount;
    public List<Tuple<Item,int>> results;
    int ResultIndex = 0;
    CraftingResult ResultSlot;

    public static string CraftingError;
    private AudioSource audioSource;
    public AudioClip craftingSound;
    private bool soundPlayed = false;

    public Recipe provided;

    public static Crafting Instance;

    public enum ComponentType
    {
        Component,
        Filling,
        Fuel
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        inventory = Inventory.Instance;
        slots = FindObjectsByType<CraftingSlot>(FindObjectsSortMode.None);
        ResultSlot = FindAnyObjectByType<CraftingResult>();
        results = new();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        //CheckCrafting();
    }


    public void CheckCrafting()
    {
        Item.temperature temperature = Item.temperature.Normal;
        List<Item> items = new();
        results.Clear();
        result = null;
        ResultCount = 1;

        foreach (var slot in slots)
        {

            if (slot.IsSet)
            {
                var item = slot.item;
                if (slot.type == ComponentType.Fuel)
                {
                    temperature = item.BurningTemperature;
                    continue;
                }
                else
                    items.Add(item);
            }
        }

        provided = new Recipe(items, temperature);

        foreach (var recipe in Inventory.Instance.recipes)
        {
            if (recipe.IsValid(provided))
            {
                if (!audioSource.isPlaying && !soundPlayed)
                {
                    audioSource.PlayOneShot(craftingSound);
                    soundPlayed = true;
                }

                results.Add(
                    new Tuple<Item,int>(inventory.FindItemByName(recipe.ItemName), recipe.ItemCount)
                );

            }
        }

        if(results.Count > 0)
        {
            result = results[0].Item1;
            ResultCount = results[0].Item2;
            ResultIndex = 0;
        }


        ResultSlot.SetItem(result);
        ResultSlot.ItemCount = ResultCount;
    }

    public void NextResult()
    {
        if (results.Count == 0)
            return;

        ResultIndex++;
        if(ResultIndex >= results.Count)
            ResultIndex = 0;

        result = results[ResultIndex].Item1;
        ResultCount = results[ResultIndex].Item2;

        ResultSlot.SetItem(result);
        ResultSlot.ItemCount = ResultCount;
    }

    public void PreviousResult()
    {
        if (results.Count == 0)
            return;

        ResultIndex--;
        if (ResultIndex < 0)
            ResultIndex = results.Count - 1;

        result = results[ResultIndex].Item1;
        ResultCount = results[ResultIndex].Item2;

        ResultSlot.SetItem(result);
        ResultSlot.ItemCount = ResultCount;
    }


    public void ConfirmCrafting()
    {
        if(result != null)
        {
            inventory.AddPlayerItem(result.Name,ResultCount);
            soundPlayed = false;

            foreach (var slot in slots)
            {
                slot.EmptySlot();
            }

            CheckCrafting();
        }
    }

}
