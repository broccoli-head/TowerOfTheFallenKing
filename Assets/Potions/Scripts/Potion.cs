using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Xml;
using Unity.Mathematics;
using UnityEngine;
using Newtonsoft.Json;
using static Item;

[System.Serializable]
public class Potion : Item
{
    public List<RequiredAttributes> Attributes;

    [Header("Basic")]
    public float Damage;
    public float OnContactDamage;
    public DamageType damageType;
    public DamagePlace damagePlace;
    public bool EnemyOnly;


    public PotionObject potion;
    public GameObject potionEffect;

    public float Time;

    //zmienne ustalajace jak ma dzialac efekt
    [Header("Additional")]
    public bool cleanse;
    public bool Heal;
    public float HealAmount;
    public bool DestroyAfterHeal;

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

    public override ItemType type => ItemType.Potion;

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

    public override StateOfMatter GetStateOfMatter()
    {
        return StateOfMatter.Liquid;
    }

    public override List<RequiredAttributes> GetAttributes()
    {
        return Attributes;
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