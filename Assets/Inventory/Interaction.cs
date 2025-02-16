using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Interaction 
{
    public string PotionName;
    public PotionInteraction[] interactions;

    public Component find(string name)
    {
        foreach (var interaction in interactions)
        {
            if (interaction.PotionName.Equals(name, StringComparison.OrdinalIgnoreCase))
                return interaction.script;
        }
        return null;
    }
}

[System.Serializable]
public class PotionInteraction
{
    public string PotionName;
    public Component script;
}