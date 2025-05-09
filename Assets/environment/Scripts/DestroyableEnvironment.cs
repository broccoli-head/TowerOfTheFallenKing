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

    public List<DropItem> Items;
    public float ItemPlacementMaxDistance = 1f;

    private bool itemsGiven = false;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip destroySound;

    List<OnLeaveDamage> OnLeaveDamage = new List<OnLeaveDamage>();


    void Start()
    {
        if(durability ==  0f)
            durability = 0.01f;

        audioSource = GetComponent<AudioSource>();
    }

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
            if(this.gameObject.name == "Chest")
                ChestSpawner.claimedChests.Add(Level.CurrentlyOnRoom);

            if (!itemsGiven)
            {
                List<GameObject> obj = new List<GameObject>();

                //Losuje itemy z listy
                foreach (var item in Items)
                {
                    int rand = Random.Range(0, 100);

                    if(item.Chance >= rand)
                        obj.Add(PickUp.Object(item.Name));
                }


                // Rozrzuca wybrane itemy
                for (int i = 0; i < obj.Count; i++)
                {
                    float angle = Random.Range(0f, Mathf.PI * 2);

                    float distance = Random.Range(0f, ItemPlacementMaxDistance);

                    Vector2 position = new Vector2(
                        transform.position.x + (distance * Mathf.Cos(angle)),
                        transform.position.y + (distance * Mathf.Sin(angle))
                    );

                    obj[i].transform.position = position;
                }

                itemsGiven = true;
            }

            //puszcza dźwiek niszczenia obiektu
            if(audioSource != null)
                if (!audioSource.isPlaying)
                    audioSource.PlayOneShot(destroySound);

            //ukrywa obiekt na czas trwania dźwięku
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            
            //usuwa obiekt po skonczeniu dzwieku
            if(destroySound != null)
                Destroy(gameObject, destroySound.length);
            else
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

    public void Damage(float Damage)
    {
        durability -= Damage;
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


    [System.Serializable]
    public class DropItem
    {
        public string Name;
        [Range(0,100)] public int Chance;
    }
}
