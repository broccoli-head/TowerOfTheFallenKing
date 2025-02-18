using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using UnityEngine;
using static Potion;

public class DestroyableEnvironment : MonoBehaviour, ReciveDamage
{

    public float durability;
    public bool ExcludeBiologicalDamage;

    public string[] Items;
    public float ItemPlacementMaxDistance = 1f;

    [Header("Randomize")]
    public bool random;
    public int ItemCount;

    List<OnLeaveDamage> OnLeaveDamage = new List<OnLeaveDamage>();


    void Update()
    {
        foreach(var damage in OnLeaveDamage)
        {
            durability -= damage.Damage * Time.deltaTime;
            damage.Time -= Time.deltaTime;

            if (damage.Time <= 0)
            {
                Destroy(damage.Effect);
                OnLeaveDamage.Remove(damage);
            }
        }
        if(durability <= 0)
        {
            List<GameObject> obj = new List<GameObject>();
            // Okreslona liczba losowych itemow z listy
            if (random)
            {
                for (int i = 0;i < ItemCount; i++)
                {
                    string item = Items[Random.Range(0, Items.Length)];
                    obj.Add(PickUp.Object(item));
                }
            }
            // Wszystkie elementy z listy
            else
            {
                foreach(var item in Items)
                {
                    obj.Add(PickUp.Object(item));
                }
            }

            for (int i = 0; i < obj.Count; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2);

                float distance = Random.Range(0f, ItemPlacementMaxDistance);

                Vector2 position = new Vector2(
                    transform.position.x + distance * Mathf.Cos(angle),
                    transform.position.y + distance * Mathf.Sin(angle)
                );

                Instantiate(obj[i], position, Quaternion.identity);
            }

            Destroy(gameObject);
        }

    }

    void ReciveDamage.Damage(float Damage, Potion.DamageType DamageType, Potion.DamagePlace DamagePlace, bool EnemyOnly)
    {
        // Mozliwosc ignorowania Biological damage np. trucizna 
        if(!ExcludeBiologicalDamage || (DamageType != Potion.DamageType.Biological && DamageType != Potion.DamageType.MagicBiological))
        {
            durability -= Damage;
        }
    }

    void ReciveDamage.Damage(float DPS, float Time, Potion.DamageType DamageType, Potion.DamagePlace DamagePlace, GameObject EffectObject, bool EnemyOnly)
    {
        if (!ExcludeBiologicalDamage || (DamageType != Potion.DamageType.Biological && DamageType != Potion.DamageType.MagicBiological))
        {
            OnLeaveDamage damage = new OnLeaveDamage();
            damage.Damage = DPS;
            damage.Time = Time;
            damage.Effect = Instantiate(EffectObject, transform);

            OnLeaveDamage.Add(damage);
        }
    }

    public void Expose(List<ExpositionData> ExpositionOverTime)
    {
        // elementy otoczenia nie otrzymoja efektu exposed
        return;
    }

    void ReciveDamage.AddCleanse()
    {
        foreach(var damage in OnLeaveDamage)
        {
            Destroy(damage.Effect);
        }
        OnLeaveDamage.Clear();
    }
}
