using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public abstract Sprite GetSprite();
    public abstract bool IsPotion();
    public abstract string GetDescription();
    public abstract string GetName();
    public abstract string GetAttributeStr();

    public enum Attribute
    {
        Explosive,
        Toxic,
        flammable,
        Healing
    }

    public enum temperature
    {
        VeryLow,
        Low,
        Normal,
        High,
        VeryHigh
    }
}
