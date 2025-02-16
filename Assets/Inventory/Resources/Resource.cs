using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Resource : Item
{
    public string Name;
    public string Description;
    public Sprite sprite;
    public Item.Attribute attribute;
    public bool IsFuel;
    public Item.temperature BurningTemperature;
    public float Price;
    
    public Resource(string Name,string Description,Sprite sprite,bool IsFuel, Item.temperature BurningTemperature,float Price)
    {
        this.Name = Name;
        this.Description = Description;
        this.sprite = sprite;
        this.IsFuel = IsFuel;
        this.BurningTemperature = BurningTemperature;
        this.Price = Price;
    }

    override public bool IsPotion()
    {
        return false;
    }
    public override Sprite GetSprite()
    {
        return sprite;
    }
    public override string GetDescription()
    {
        return Description;
    }

    public override string GetName()
    {
        return Name;
    }
    public override string GetAttributeStr()
    {
        return attribute.ToString();
    }
}
