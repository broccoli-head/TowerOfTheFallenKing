using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    Inventory inventory;
    CraftingSlot[] slots;
    Dictionary<Tuple<HashSet<string>, HashSet<string>, Item.temperature>, Item> CraftingRecipes;
    Item result;
    CraftingResult ResultSlot;

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

        CraftingRecipes = new Dictionary<Tuple<HashSet<string>, HashSet<string>, Item.temperature>, Item>(new RecipeComparer());

        // Dodawanie receptur
        foreach (var recipe in inventory.recipes)
        {
            var recipeKey = new Tuple<HashSet<string>, HashSet<string>, Item.temperature>(
                new HashSet<string>(recipe.GetComponents()),
                new HashSet<string>(recipe.GetFillers()),
                recipe.FuelTemperature
            );

            if(inventory.FindItemByName(recipe.ItemName) == null)
                Debug.LogError("Item: " +  recipe.ItemName + " nie istnieje. Receptura craftowania nieprzwidlowa!");

            CraftingRecipes.Add(
                recipeKey,
                inventory.FindItemByName(recipe.ItemName)
            ); 
        }
    }

    void Update()
    {
        List<string> Components = new List<string>();
        List<string> Fillers = new List<string>();
        Item.temperature temperature = Item.temperature.Normal;

        foreach (var slot in slots)
        {
            if (slot.IsSet)
            {
                if (slot.type == ComponentType.Component)
                {
                    Components.Add(slot.item.GetAttributeStr());
                }
                if (slot.type == ComponentType.Filling)
                {
                    Fillers.Add("Filling" + slot.item.GetAttributeStr());
                }
                if (slot.type == ComponentType.Fuel)
                {
                    temperature = slot.BurningTemperature;
                }
            }
        }

        var recipeKey = new Tuple<HashSet<string>, HashSet<string>, Item.temperature>(
            new HashSet<string>(Components),
            new HashSet<string>(Fillers),
            temperature
        );

        if (CraftingRecipes.ContainsKey(recipeKey))
        {
            result = CraftingRecipes[recipeKey];
        }
        else
        {
            result = null;
        }

        //Powiadamia CraftingResult o rezultacie craftowania (Item/null)
        ResultSlot.SetPotion(result);

    }

    public void ConfirmCrafting()
    {
        if(result != null)
        {
            if (result.IsPotion())
                inventory.AddPlayerPotion(result.GetName());
            else
                inventory.AddPlayerResource(result.GetName());

            foreach(var slot in slots)
            {
                slot.EmptySlot();
            }
        }
    }


    private class RecipeComparer : IEqualityComparer<Tuple<HashSet<string>, HashSet<string>, Item.temperature>>
    {
        public bool Equals(Tuple<HashSet<string>, HashSet<string>, Item.temperature> x, Tuple<HashSet<string>, HashSet<string>, Item.temperature> y)
        {
            if (x == null || y == null)
                return false;

            return x.Item1.SetEquals(y.Item1) && x.Item2.SetEquals(y.Item2) && x.Item3 == y.Item3;
        }

        public int GetHashCode(Tuple<HashSet<string>, HashSet<string>, Item.temperature> obj)
        {
            if (obj == null)
                return 0;

            int hash = 17;

            hash = hash * 31 + obj.Item1.Aggregate(0, (acc, item) => acc + item.GetHashCode());
            hash = hash * 31 + obj.Item2.Aggregate(0, (acc, item) => acc + item.GetHashCode());
            hash = hash * 31 + obj.Item3.GetHashCode();

            return hash;
        }
    }
}
