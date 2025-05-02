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
    CraftingResult ResultSlot;

    public static string CraftingError;
    private AudioSource audioSource;
    public AudioClip craftingSound;
    private bool soundPlayed = false;


    public enum ComponentType
    {
        Component,
        Filling,
        Fuel
    }

    void Start()
    {
        inventory = Inventory.Instance;
        slots = FindObjectsByType<CraftingSlot>(FindObjectsSortMode.None);
        ResultSlot = FindAnyObjectByType<CraftingResult>();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Item.temperature temperature = Item.temperature.Normal;
        List<Item> items = new();
        result = null;
        ResultCount = 1;

        foreach (var slot in slots) {

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

        Recipe provided = new Recipe(items, temperature);

        foreach(var recipe in Inventory.Instance.recipes)
        {
            if (recipe.IsValid(provided))
            {
                if (!audioSource.isPlaying && !soundPlayed)
                {
                    audioSource.PlayOneShot(craftingSound);
                    soundPlayed = true;
                }

                result = inventory.FindItemByName(recipe.ItemName);
                ResultCount = recipe.ItemCount;
            }
        }


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
        }
    }

}
