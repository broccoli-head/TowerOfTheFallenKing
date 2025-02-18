using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLive : MonoBehaviour, ReciveDamage, Saveable 
{
    public bool load = true;
    public float HitPoints;

    private float damage = 0;
    private bool isExposed = false;
    private float Exposition;

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
        Damage(damage * Time.deltaTime);
        if(HitPoints <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Damage(float damage, Potion.DamageType DamageType, Potion.DamagePlace DamagePlace,bool EnemyOnly)
    {
        if (DamageType != Potion.DamageType.None && !EnemyOnly) { 
            if (!cleanse)
                Damage(damage);
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

    public void Damage(float dmg)
    {
        if (isExposed)
        {
            // zmienia damage na (100 + Exposition)% aktualnych obrazen 
            dmg *= 1 + (Exposition / 100f);
        }
        HitPoints -= dmg;
    }


    public void AddCleanse()
    {
        foreach (var item in effects)
        {
            Destroy(item);
        }
        damage = 0;
        Exposition = 0f;
        isExposed = false;
        cleanse = true;
    }

    public void Expose(List<ExpositionData> ExpositionOverTime)
    {
        isExposed = true;
        StartCoroutine(ChangeExposedForce(ExpositionOverTime));
    }


    public IEnumerator endDamage(float damage, float time, GameObject effect)
    {
        yield return new WaitForSeconds(time);
        this.damage -= damage;
        Destroy(effect);
        yield break;
    }

    public IEnumerator ChangeExposedForce(List<ExpositionData> oExp)
    {
        List<ExpositionData> Exp = new List<ExpositionData>(oExp);
        if (Exp.Count <= 0)
        {
            Exposition = 0f;
            isExposed = false;
            yield break;
        }
        else
        {
            Exposition = Exp[0].exposition;
            yield return new WaitForSeconds(Exp[0].time);
            Exp.RemoveAt(0);
            StartCoroutine(ChangeExposedForce(Exp));
        }

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
