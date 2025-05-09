using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class AttackHandler : MonoBehaviour
{
    Enemy enemy;
    bool AttackReady = true;
    bool SpecialAttackReady = true;

    private SpriteRenderer spriteRenderer;
    private bool flippedDuringAttack = false;
    private bool originalFlipX = false;


    void Start()
    {
        enemy = GetComponent<Enemy>();
        AttackReady = true;
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {

        // sprawdzanie czy przeciwnik powinien i czy mo¿e zaatakowac gracza
        if (enemy.IsAgresive && AttackReady && !enemy.IsFreezed())
        {

            bool attacked = false;
            
            // Gdy gracz jest w zasiêgu przeciwnika i przeciwnik ma Melee attack - u¿ywa go
            if(enemy.MeleeAttack && enemy.InRange)
            {
                // jeœli przeciwnik dysponuje specjalnym melee attack - u¿ywa go
                if (SpecialAttackReady && enemy.SpecialMeeleAttacks.Count > 0)
                {

                    //losujemy atak z listy
                    int i = Random.Range(0, enemy.SpecialMeeleAttacks.Count);
                    var attack = enemy.SpecialMeeleAttacks[i];

                    StartCoroutine(MeleeAttack(attack.Attack));
                    StartCoroutine(SpecialAttackCooldown(attack.Cooldown));

                    attacked = true;
                }
                // jesli przeciwnik nie ma specjalnego ataku - atakuje zwyk³ym
                else 
                {
                    int i = Random.Range(0, enemy.MeeleAttacks.Count);
                    var attack = enemy.MeeleAttacks[i];

                    StartCoroutine(MeleeAttack(attack));

                    attacked = true;
                }
            }
            // gdy nie uzywamy melee, przeciwnik probuje wykonac atak z dystansu
            else
            {
                bool SpecialAllAroundAttack = false;
                bool SpecialAttack = false;

                bool AllAroundAttack = false;
                bool Attack = false;

                // jeœli mamy obydwa ataki za pomoc¹ projectile, wybieramy ktore bêd¹ uzyte
                if (enemy.ProjectileAllAroundAttack && enemy.ProjectileAttack)
                {
                    // jesli jedna z list jest pusta wybieramy druga, jesli nie wybieramy losowo
                    if (enemy.SpecialAllAroundProjectiles.Count == 0)
                        SpecialAttack = true;
                    else if (enemy.SpecialProjectiles.Count == 0)
                        SpecialAllAroundAttack = true;
                    else
                    {
                        bool rand = Random.value < 0.5;
                        SpecialAttack = rand;
                        SpecialAllAroundAttack = !rand;
                    }

                    // jesli jedna z list jest pusta wybieramy druga, jesli nie wybieramy losowo
                    if (enemy.AllAroundProjectiles.Count == 0)
                        Attack = true;
                    else if (enemy.Projectiles.Count == 0)
                        AllAroundAttack = true;
                    else
                    {
                        bool rand = Random.value < 0.5;
                        Attack = rand;
                        AllAroundAttack = !rand;
                    }
                }
                else if (enemy.ProjectileAttack) 
                {
                    SpecialAttack = true;
                    Attack = true;
                }else if (enemy.ProjectileAllAroundAttack)
                {
                    SpecialAllAroundAttack = true;
                    AllAroundAttack = true;
                }


                if (SpecialAllAroundAttack && !attacked && enemy.SpecialAllAroundProjectiles.Count > 0 && SpecialAttackReady)
                {
                    //losujemy atak z listy
                    int i = Random.Range(0, enemy.SpecialAllAroundProjectiles.Count);
                    var attack = enemy.SpecialAllAroundProjectiles[i];

                    int count = Random.Range(attack.Attack.MinProjectileCount, attack.Attack.MaxProjectileCount);

                    AllAroundProjectileAttack(attack.Attack.projectile, count);
                    
                    StartCoroutine(SpecialAttackCooldown(attack.Cooldown));
                    attacked = true;
                }
                else if (SpecialAttack && !attacked && enemy.SpecialProjectiles.Count > 0 && SpecialAttackReady)
                {
                    //losujemy atak z listy
                    int i = Random.Range(0, enemy.SpecialProjectiles.Count);
                    var attack = enemy.SpecialProjectiles[i];
                    Vector2 direction = (enemy.Player.transform.position - transform.position).normalized;

                    StartCoroutine(ProjectileAttack(attack.Attack, direction));
                    StartCoroutine(SpecialAttackCooldown(attack.Cooldown));

                    attacked = true;
                }
                else if (AllAroundAttack && !attacked && enemy.AllAroundProjectiles.Count > 0)
                {
                    //losujemy atak z listy
                    int i = Random.Range(0, enemy.AllAroundProjectiles.Count);
                    var attack = enemy.AllAroundProjectiles[i];

                    int count = Random.Range(attack.MinProjectileCount, attack.MaxProjectileCount);

                    AllAroundProjectileAttack(attack.projectile, count);
                    attacked = true;
                }
                else if (Attack && !attacked && enemy.Projectiles.Count > 0)
                {
                    //losujemy atak z listy
                    int i = Random.Range(0, enemy.Projectiles.Count);
                    var attack = enemy.Projectiles[i];
                    Vector2 direction = (enemy.Player.transform.position - transform.position).normalized;

                    StartCoroutine(ProjectileAttack(attack, direction));

                    attacked = true;
                }


            }

            // jeœli zosta³ wykonany jakikolwiek atak, ustawiamy cooldown
            if (attacked)
            {
                if (spriteRenderer != null && enemy.Player != null)
                {
                    //obraca w strone gracza
                    originalFlipX = spriteRenderer.flipX;
                    Vector2 toPlayer = enemy.Player.transform.position - transform.position;

                    if (toPlayer.x < 0)
                        spriteRenderer.flipX = true;
                    else
                        spriteRenderer.flipX = false;

                    flippedDuringAttack = true;
                }

                if (TryGetComponent<Animator>(out var anim))
                {
                    try
                    {
                        anim.SetTrigger("Attack");
                    }
                    catch {}
                }

                StartCoroutine(AttackCooldown(enemy.AttackFrequency));
                StartCoroutine(FlipAfterAttack());
            }


            if (attacked && enemy.Teleportation)
            {
                int rand = Random.Range(0, 100);
                if (rand <= enemy.TeleportAfterAttackChance)
                {
                    if (enemy.TryGetComponent<TeleportationMovement>(out var movement))
                        movement.Teleport();
                }
                    
            }  
        }
    }


    public IEnumerator MeleeAttack(MeleeStats stats)
    {
        //opoznienie ataku
        yield return new WaitForSeconds(stats.AttackDelay);

        //zadawanie obrazen
        Collider2D[] Attacked = Physics2D.OverlapCircleAll(transform.position, enemy.Range + (GetComponent<Collider2D>().bounds.size.y / 2));
        foreach (Collider2D obj in Attacked)
        {
            //przeciwnik nie atakuje samego siebie
            if (obj.gameObject == gameObject) continue;

            // jesli frendly fire jest wylaczony pomija innych przeciwnikow
            if (!stats.FriendlyFire)
                if (obj.TryGetComponent<Enemy>(out Enemy en))
                    continue;

            //proboje pobrac component implemetujacy ReciveDamage jesli go znajdzie zadaje obrazenia
            var target = Helper.GetInterfaceComponent<ReciveDamage>(obj.gameObject);
            if (target != null)
            {
                target.Damage(stats.AttackDamage + enemy.AdditionalDamage, stats.DamageType, Potion.DamagePlace.Zone, false);
            }
        }

        yield break;
    }

    
    public IEnumerator ProjectileAttack(Projectile projectile, Vector2 direction)
    {
        if (projectile == null)
            yield return -1;

        // Spawnuje obiekt na scenie
        GameObject obj = Instantiate(projectile.gameObject,transform.position,Quaternion.identity);

        SetUpProjectile(obj);


        // Wystrzeliwuje pocisk w okreœlonym kierunku
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        rb.AddForce( direction * projectile.Speed,ForceMode2D.Impulse);
        
        yield break;
    }

    public void AllAroundProjectileAttack(Projectile prefab, int ProjectileCount)
    {
        float step = 360 / ProjectileCount;

        for(int i = 0; i < ProjectileCount; i++)
        {
            Vector2 direction = Helper.RotateVector(Vector2.up, step * i);
            StartCoroutine(
                ProjectileAttack(prefab,direction)
            );
        }

        return;
    }


    void SetUpProjectile(GameObject projectile)
    {
        Projectile stats = projectile.GetComponent<Projectile>();
        stats.AttackDamage += enemy.AdditionalDamage;

        //Ustawia wszystkie komponenty potrzebne do dzia³ania Projectile
        Rigidbody2D rb;
        if (!projectile.TryGetComponent<Rigidbody2D>(out rb))
            rb = projectile.AddComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.drag = stats.Drag / 10;

        Collider2D collider;
        if (!projectile.TryGetComponent<Collider2D>(out collider))
            collider = projectile.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        SpriteRenderer spriteRenderer;
        if (!projectile.TryGetComponent<SpriteRenderer>(out spriteRenderer))
            spriteRenderer = projectile.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 6;
    }


    public IEnumerator AttackCooldown(float cooldown)
    {
        AttackReady = false;
        yield return new WaitForSeconds(cooldown);
        AttackReady = true;
        yield break;
    }

    public IEnumerator SpecialAttackCooldown(float cooldown)
    {
        SpecialAttackReady = false;
        yield return new WaitForSeconds(cooldown);
        SpecialAttackReady = true;
        yield break;
    }

    private IEnumerator FlipAfterAttack()
    {
        //czeka a¿ animacja bicia siê zakoñczy
        yield return new WaitForSeconds(0.4f);

        if (flippedDuringAttack && spriteRenderer != null)
        {
            spriteRenderer.flipX = originalFlipX;
            flippedDuringAttack = false;
        }
    }



    [System.Serializable]
    public class MeleeStats
    {
        public float AttackDamage;
        public Potion.DamageType DamageType;
        public float AttackDelay;
        public bool FriendlyFire;
    }


    [System.Serializable]
    public class AllAroundProjectile
    {
        public Projectile projectile;
        public int MinProjectileCount;
        public int MaxProjectileCount;
    }

    [System.Serializable]
    public class SpecialAttack<T> 
    {
        public float Cooldown;
        public T Attack;
    }



}
