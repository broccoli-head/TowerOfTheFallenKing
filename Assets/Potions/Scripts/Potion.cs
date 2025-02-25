using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Xml;
using Unity.Mathematics;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class Potion : Item
{
    [Header("Basic")]
    public string Name;
    public string description;
    public float Damage;
    public float OnContactDamage;
    public DamageType damageType;
    public DamagePlace damagePlace;
    public bool EnemyOnly;


    public PotionObject potion;
    public GameObject potionEffect;
    public Sprite sprite;

    public float Time;

    //zmienne ustalajace jak ma dzialac efekt
    [Header("Additional")]
    public bool cleanse;

    [Space(10)]
    public bool freeze;
    public float freezeTime;

    [Space(10)]
    public bool speedChange;
    public float speedTime;
    public float speedMultiplier;

    [Space(10)]
    public bool ExposedEffect;
    public List<ExpositionData> ExpositionOverTime;
    
    [Space(10)]
    public Shape shape;
    public int BeamLength;
    public float BeamSpawnTime;
    public Explosion explosion;
    public OnLeaveDamage onLeaveDamage;


    public enum DamageType
    {
        None,
        Magic,
        Physical,
        Biological,
        MagicPhysical,
        MagicBiological,
        PhysicBiological
    }
    public enum Shape
    {
        Circular,
        Beam
    }
    public enum DamagePlace
    {
        Ground,
        Zone
    }

    override public bool IsPotion()
    {
        return true;
    }
    public override Sprite GetSprite()
    {
        return sprite;
    }

    public override string GetDescription()
    {
        return description;
    }
    public override string GetName()
    {
        return Name;
    }
    public override string GetAttributeStr()
    {
        return Name;
    }

}


[Serializable]
public class OnLeaveDamage
{
    public bool HaveOnLeaveDamage;
    public float Damage;
    public float Time;
    public GameObject Effect;
}


[System.Serializable]
public class Explosion
{
    public bool IsExplosive;
    public float Force;
    public GameObject Effect;
    public float EffectTime;
}

[System.Serializable]
public class ExpositionData
{
    public float time;
    public int exposition;
}