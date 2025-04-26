using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string Name;
    [TextArea] public string Description;
    public Sprite sprite;
    public bool IsFuel;
    public Item.temperature BurningTemperature;
    public float Price;

    public virtual ItemType type => ItemType.Item;

    public Sprite GetSprite()
    {
        return sprite;
    }

    public virtual List<RequiredAttributes> GetAttributes()
    {
        return new();
    }

    public virtual StateOfMatter GetStateOfMatter()
    {
        return StateOfMatter.None;
    }

    public enum Attribute
    {
        Holy,
        Corrosive,
        Diluent,
        Flammable,
        Transparent,
        Sharp,
        Icy,
        Explosive,
        Energizing

    }

    public enum StateOfMatter
    {
        Solid,
        Dusty,
        Liquid,
        None
    }


    public enum temperature
    {
        VeryLow,
        Low,
        Normal,
        High,
        VeryHigh
    }

    public enum ItemType
    {
        Item,
        Potion,
        Resource
    }
}