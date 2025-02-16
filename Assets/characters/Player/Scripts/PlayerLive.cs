using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLive : MonoBehaviour, ReviceDamage, Saveable 
{
    public bool load = true;

    public float HitPoints;
    float damage = 0;

    List<GameObject> effects = new List<GameObject>();
    bool cleanse = false;

    [HideInInspector][Min(1.0f)] public float StartHP;

    void Start()
    {
        if (load)
            Load();
        else
            StartHP = HitPoints;
    }

    void Update()
    {
        if (damage < 0)
            damage = 0;
        HitPoints -= damage * Time.deltaTime;
        if(HitPoints <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Damage(float Damage, Potion.DamageType DamageType, Potion.DamagePlace DamagePlace,bool EnemyOnly)
    {
        if (DamageType != Potion.DamageType.None && !EnemyOnly) { 
            if (!cleanse)
                HitPoints -= Damage;
            else
                cleanse = false;
        }
    }

    public void Damage(float DPS, float Time, Potion.DamageType DamageType, Potion.DamagePlace DamagePlace, GameObject EffectObject, bool EnemyOnly)
    {
        if(DamageType != Potion.DamageType.None)
        {
            if(!cleanse && !EnemyOnly)
            {
                damage += DPS;
                var effect = Instantiate(EffectObject, transform);
                effects.Add(effect);
                try
                {
                    StartCoroutine(endDamage(DPS, Time, effect));
                }
                catch
                {
                }
                
            }else 
                cleanse = false;
        }
        
    }


    public void AddCleanse()
    {
        foreach (var item in effects)
        {
            Destroy(item);
        }
        damage = 0;
        cleanse = true;
    }


    public IEnumerator endDamage(float damage, float time, GameObject effect)
    {
        yield return new WaitForSeconds(time);
        this.damage -= damage;
        Destroy(effect);
        yield break;
    }

    public void Save()
    {
        SaveManager.SaveVar<float>("HitPoints",HitPoints);
        SaveManager.SaveVar<float>("StartHP", StartHP);
    }

    public void Load() 
    {
        HitPoints = SaveManager.LoadVar<float>("HitPoints");
        StartHP = SaveManager.LoadVar<float>("StartHP");
    }

}
