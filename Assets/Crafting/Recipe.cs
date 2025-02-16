using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public string ItemName;

    public List<CraftingElement> Components;
    public List<CraftingElement> Fillers;
    public Item.temperature FuelTemperature;

    public List<string> GetComponents()
    {
        List<string> components = new List<string>();
        foreach (var component in Components)
        {
            components.Add(component.ToString());
        }
        return components;
    }

    public List<string> GetFillers()
    {
        List<string> fillers = new List<string>();
        foreach (var filler in Fillers)
        {
            fillers.Add(filler.ToString());
        }
        return fillers;
    }
}

[System.Serializable]
public class CraftingElement
{
    public enum DataType
    {
        Attribute,
        Potion
    }

    public DataType selectedDataType;

    public Item.Attribute Attribute;
    public string PotionName;

    //nadpisuje metodê to string, w zale¿noœci od wybranego typy danych zwraca name potki lub atrybut
    public override string ToString()
    {
        if (selectedDataType == DataType.Attribute)
            return Attribute.ToString();
        else
        {
            Inventory inv = GameObject.FindFirstObjectByType<Inventory>();
            if (inv.FindPotionByName(PotionName) == null)
            {
                Debug.LogError("Nie znaleziono Mikstury: " +  PotionName + "! Receptura craftowania nieprawid³owa");
            }
            return PotionName;
        }
            
    }
}
