using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour, ReciveDamage, ReciveSpeedChange
{
    [Header("Detection")]
    [Min(1.0f)] public float HearingDistance;
    public float FieldOfViev;
    public float VievDistance;
    public float MemoryTime;

    [Header("Defensive")]
    public float HP;
    public EnemyType Type;
    public bool Flying;
    public DamageModificator[] DamageModificators;

    [Header("Offensive")]
    public AttackMode attackMode;
    public AttackHandler.MeleeStats MeleeStats;
    public AttackHandler.ProjectileStats ProjectileStats;


    [Header("Movement")]
    public bool looping;
    public float speed; //predkosc podczas chodzenia 
    public float chaseSpeed; //predkosc podczas gonienia
    public Move[] moves;
    public bool Teleportation;
    public float TeleportAwayDistance;
    [Range(0,100)] public int TeleportAfterAttackChance;



    [HideInInspector] public float FreezeTime;
    [HideInInspector] public Vector2 FacingDirection = Vector2.down;
    [HideInInspector] public GameObject Player;
    [HideInInspector] public bool IsAgresive { get; private set; } = false;
    [ReadOnly] public bool PlayerDetected = false;

    private List<GameObject> effects = new();
    private NPCMovement NpcMovement;
    private ChaseMovement Chase;
    private Detection detection;
    private AttackHandler attackHandler;
    private Rigidbody2D rb;

    [ReadOnly] private bool DamageTaken = false;
    private bool cleanse = false;
    private float damage = 0;
    private bool speedChanged = false;
    private float speedTime;
    private float speedMultiplier;
    
    private bool isKnockedBack = false;
    private float knockbackTimer = 0;
    private Vector2 knockbackDirection;
    private bool isExposed = false;
    private float Exposition;


    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        rb = GetComponent<Rigidbody2D>();
        rb.drag = 0.6f;

        if(speed > 0 && moves.Length > 0)
        {
            NpcMovement = gameObject.AddComponent<NPCMovement>();
            NpcMovement.enabled = !IsAgresive;
        }
        if(chaseSpeed > 0)
        {
            Chase = gameObject.AddComponent<ChaseMovement>();
            Chase.enabled = IsAgresive;
        }
        if (Teleportation)
        {
            gameObject.AddComponent<TeleportationMovement>().RunAwayDistance = TeleportAwayDistance;

        }

        detection = gameObject.AddComponent<Detection>();
        attackHandler = gameObject.AddComponent<AttackHandler>();

        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        //Zycie przeciwnika
        if(damage < 0 ) damage = 0;
        if (damage > 0)
        {
            Damage(damage * Time.deltaTime);
        }
        if (HP < 0) 
            Destroy(gameObject);


        //Sprawdzanie czy przeciwnik powinien atakowaæ gracza
        if (attackMode == AttackMode.Always)
            IsAgresive = true;
        else if (attackMode == AttackMode.OnSpot)
        {
            if (PlayerDetected)
                IsAgresive = true;
            else IsAgresive = false;
        }
        else if(attackMode == AttackMode.OnDamage)
        {
            if (PlayerDetected && DamageTaken)
                IsAgresive = true;
            else IsAgresive = false;
        }

        //Ustawienie sposobu poruszania przeciwnika (gonienie / patrolowanie)
        if(Chase != null)
            Chase.enabled = IsAgresive;
        if(NpcMovement != null) 
            NpcMovement.enabled = !IsAgresive;


        //speed change effect
        if (speedTime > 0.0f)
        {
            speedTime -= Time.deltaTime;
        }

        if (speedTime < 0.0f && speedChanged)
        {
            speed /= speedMultiplier;
            chaseSpeed /= speedMultiplier;
            speedChanged = false;
            speedTime = 0.0f;
        }


        //knockback do sztyletowania
        if (isKnockedBack)
        {                          
            if (knockbackTimer > 0)
            {
                knockbackTimer -= Time.deltaTime;
                rb.MovePosition(rb.position + knockbackDirection * Time.deltaTime);
            }
            else
            {
                isKnockedBack = false;
            }
        }
        
    }

    // Odpowiada za jednorazowe otrzymanie Damage
    public void Damage(float damage, Potion.DamageType DamageType, Potion.DamagePlace DamagePlace, bool EnemyOnly)
    {
        DamageTaken = true;
        if ( (!Flying || DamagePlace == Potion.DamagePlace.Zone)  &&  DamageType != Potion.DamageType.None)
        {
            if (cleanse)
            {
                cleanse = false;
                return;
            }             
            foreach(var Modificator in DamageModificators)
            {
                if(DamageType == Modificator.DamageType)
                {
                    damage *= (Modificator.modificator / 100f);
                    Damage(damage);
                    break;
                }
            }
            
        }
    }

    // Odpowiada za otrzymywanie Damage przez dluzszy okres w wyniku np. podpalenia
    public void Damage(float DPS, float Time, Potion.DamageType DamageType, Potion.DamagePlace DamagePlace, GameObject EffectObject, bool EnemyOnly)
    {
        DamageTaken = true;
        if (!Flying || DamagePlace == Potion.DamagePlace.Zone)
        {
            if (cleanse)
            {
                cleanse = false;
                return;
            }
            foreach (var Modificator in DamageModificators)
            {
                if (DamageType == Modificator.DamageType)
                {
                    DPS *= (Modificator.modificator / 100f);
                    damage += DPS;
                    break;
                }
            }
            EffectObject = Instantiate(EffectObject,transform);
            effects.Add(EffectObject);
            StartCoroutine(EndOnLeaveDamage(EffectObject,Time,DPS));
        }        
    }

    // Odpowiada za jednorazowe otrzymanie Damage, bez parametrow - np. do sztyletu
    public void Damage(float Damage)
    {
        DamageTaken = true;
        if (isExposed) {
            // zmienia damage na (100 + Exposition)% aktualnych obrazen 
            Damage *= 1 + (Exposition / 100f);
        }
        HP -= Damage;
    }


    // Dodaje efekt Cleanse dla przeciwnika 
    // Niweluje otrzymywany Damage oraz jednorazowo zapobiega otrzymaniu obrazen w przyszlosci
    public void AddCleanse()
    {
        foreach (var item in effects)
        {
            Destroy(item);
        }
        damage = 0;
        cleanse = true;
        Exposition = 0f;
        isExposed = false;
    }

    public void ChangeSpeed(float speedMultiplier)
    {
        if (!speedChanged)
        {
            this.speedMultiplier = speedMultiplier;
            speed *= speedMultiplier;
            chaseSpeed *= speedMultiplier;
            speedChanged = true;
        }
    }
    public void ChangeSpeed(float speedMultiplier, float speedTime)
    {
        ChangeSpeed(speedMultiplier);
        this.speedTime = speedTime;
    }

    public void ChangeSpeedOnExit(float speedTime)
    {
        this.speedTime = speedTime;
    }


    public void ApplyKnockback(Vector2 direction, float force, float knockbackDuration, float freezeDuration)
    {
        isKnockedBack = true;

        knockbackDirection = direction * force;
        knockbackTimer = knockbackDuration;
        FreezeTime += freezeDuration;
    }

    
    public void Expose(List<ExpositionData> ExpositionOverTime)
    {
        isExposed = true;
        StartCoroutine(ChangeExposedForce(ExpositionOverTime));
    }

    public bool IsFreezed()
    {
        return FreezeTime > 0;
    }

    public IEnumerator EndOnLeaveDamage(GameObject EffectObject,float time,float damage)
    {
        yield return new WaitForSeconds(time - 1f);
        //play animation
        yield return new WaitForSeconds(1);
        this.damage -= damage;
        Destroy(EffectObject);
        yield break;
    }

    public IEnumerator ChangeExposedForce(List<ExpositionData> Exp)
    {
        if(Exp.Count <= 0)
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

    public enum EnemyType
    {
        Human,
        Machine,
        Spirit,
        HumanMachine,
        HumanSpirit,
        SpiritMachine
    }

    public enum AttackMode
    {
        Always,
        OnSpot,
        OnDamage,
        Never
    }
}

[System.Serializable]
public class DamageModificator
{
    public Potion.DamageType DamageType;
    public float modificator;
}