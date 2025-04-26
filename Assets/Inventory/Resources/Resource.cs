using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Resource : Item
{
    public List<RequiredAttributes> Attributes;
    public Item.StateOfMatter stateOfMatter;
    public override ItemType type => ItemType.Resource;

    public override List<RequiredAttributes> GetAttributes()
    {
        return Attributes;
    }

    public override StateOfMatter GetStateOfMatter()
    {
        return stateOfMatter;
    }
}