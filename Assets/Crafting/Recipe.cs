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
    [Min(1)]public int ItemCount = 1;
    public List<RequiredAttributes> Attributes;
    public List<RequiredStatesOfMatter> StatesOfMatter;
    public List<string> Items;
    public Item.temperature FuelTemperature;


    public Recipe(List<Item> items, Item.temperature temperature)
    {
        FuelTemperature = temperature;

        // inicjacja obiektów
        Items = new();
        Attributes = new();
        StatesOfMatter = new();

        foreach (Item item in items)
        {
            Items.Add(item.Name);

            foreach (RequiredAttributes attribute in item.GetAttributes())
            {
                var existingAttribute = Attributes.Find(a => a.attribute == attribute.attribute);

                if (existingAttribute != null)
                {
                    existingAttribute.value += attribute.value;
                }
                else
                {
                    Attributes.Add(new RequiredAttributes
                    {
                        attribute = attribute.attribute,
                        value = attribute.value
                    });
                }
            }

            var state = item.GetStateOfMatter();
            var existingState = StatesOfMatter.Find(s => s.stateOfMatter == state);

            if (existingState != null)
            {
                existingState.amount += 1;
            }
            else
            {
                StatesOfMatter.Add(new RequiredStatesOfMatter
                {
                    stateOfMatter = state,
                    amount = 1 
                });
            }
        }
    }

    public bool IsValid(Recipe other)
    {
        // Sprawdzenie temperatury
        if (this.FuelTemperature != other.FuelTemperature)
            return false;

        // Sprawdzenie atrybutów
        foreach (var requiredAttr in this.Attributes)
        {
            var attrInOther = other.Attributes.Find(a => a.attribute == requiredAttr.attribute);
            if (attrInOther == null || attrInOther.value < requiredAttr.value)
                return false;
        }

        // Sprawdzenie stanów skupienia
        foreach (var requiredState in this.StatesOfMatter)
        {
            var stateInOther = other.StatesOfMatter.Find(s => s.stateOfMatter == requiredState.stateOfMatter);
            if (stateInOther == null || stateInOther.amount < requiredState.amount)
                return false;
        }

        // Sprawdzenie konkretnych itemow (czy zawiera wszystkie wymagane itemy)
        foreach (var item in this.Items)
        {
            if (!other.Items.Contains(item))
                return false;
        }

        return true;
    }



}

[System.Serializable]
public class RequiredAttributes
{
    public Item.Attribute attribute;
    public int value;
}

[System.Serializable]
public class RequiredStatesOfMatter
{
    public Item.StateOfMatter stateOfMatter;
    public int amount;
}