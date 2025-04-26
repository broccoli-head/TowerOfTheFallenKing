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
    CraftingResult ResultSlot;

    public static string CraftingError;
    private AudioSource audioSource;
    public AudioClip craftingSound;

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
                if(!audioSource.isPlaying)
                    audioSource.PlayOneShot(craftingSound);

                result = inventory.FindItemByName(recipe.ItemName);
            }
        }


        ResultSlot.SetItem(result);

    }

    public void ConfirmCrafting()
    {
        if(result != null)
        {
            if (result.type == Item.ItemType.Potion)
                inventory.AddPlayerPotion(result.Name);
            else
                inventory.AddPlayerResource(result.Name);


            foreach (var slot in slots)
            {
                slot.EmptySlot();
            }
        }
    }

}
