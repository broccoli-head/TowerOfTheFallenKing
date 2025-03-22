using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class AttackHandler : MonoBehaviour
{

    Enemy enemy;
    bool AttackReady = true;
<<<<<<< HEAD
    MeleeStats MStats;
    ProjectileStats PStats;
=======
>>>>>>> 0b0c13f (naprawa unity i repo)

    [System.Serializable]
    public struct MeleeStats 
    {
        public bool MeleAttack;
        public float AttackDamage;
        public Potion.DamageType DamageType;
        public float AttackFrequency;
        public float AttackDistance;
<<<<<<< HEAD
        public bool InRange;
=======
        [ReadOnly] public bool InRange;
>>>>>>> 0b0c13f (naprawa unity i repo)
        public float AttackDelay;
        public bool FriendlyFire;
    }

    [System.Serializable]
    public struct ProjectileStats 
    {
        public bool ProjectileAttack;
        public GameObject Projectile;
        public float AttackDamage;
        public Potion.DamageType DamageType;
        public float AttackFrequency;
        public float Speed;
        public float LiveTime;
        public float Drag;
    }


    void Start()
    {
        enemy = GetComponent<Enemy>();
<<<<<<< HEAD
        MStats = enemy.MeleeStats;
        PStats = enemy.ProjectileStats;
=======
        AttackReady = true;
>>>>>>> 0b0c13f (naprawa unity i repo)
    }

    void Update()
    {
        if (enemy.IsAgresive && AttackReady)
        {
<<<<<<< HEAD
            bool attacked = false;
            if(MStats.MeleAttack && MStats.InRange)
=======

            bool attacked = false;
            if(enemy.MeleeStats.MeleAttack && enemy.MeleeStats.InRange)
>>>>>>> 0b0c13f (naprawa unity i repo)
            {
                StartCoroutine(MeleeAttack());
                attacked = true;
            }
<<<<<<< HEAD
            else if (PStats.ProjectileAttack)
=======
            else if (enemy.ProjectileStats.ProjectileAttack)
>>>>>>> 0b0c13f (naprawa unity i repo)
            {
                Vector2 direction = (enemy.Player.transform.position - transform.position).normalized;
                StartCoroutine(ProjectileAttack(direction));
                attacked = true;
            }

<<<<<<< HEAD
=======

>>>>>>> 0b0c13f (naprawa unity i repo)
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
;
    }


    public IEnumerator MeleeAttack()
    {
<<<<<<< HEAD
        
        AttackReady = false;
        //opoznienie ataku
        yield return new WaitForSeconds(MStats.AttackDelay);

        //zadawanie obrazen
        Collider2D[] Attacked = Physics2D.OverlapCircleAll(transform.position, MStats.AttackDistance + (GetComponent<Collider2D>().bounds.size.y / 2));
=======

        AttackReady = false;
        //opoznienie ataku
        yield return new WaitForSeconds(enemy.MeleeStats.AttackDelay);

        //zadawanie obrazen
        Collider2D[] Attacked = Physics2D.OverlapCircleAll(transform.position, enemy.MeleeStats.AttackDistance + (GetComponent<Collider2D>().bounds.size.y / 2));
>>>>>>> 0b0c13f (naprawa unity i repo)
        foreach (Collider2D obj in Attacked)
        {
            //przeciwnik nie atakuje samego siebie
            if (obj.gameObject == gameObject) continue;

            // jesli frendly fire jest wylaczony pomija innych przeciwnikow
<<<<<<< HEAD
            if (!MStats.FriendlyFire)
=======
            if (!enemy.MeleeStats.FriendlyFire)
>>>>>>> 0b0c13f (naprawa unity i repo)
                if (obj.TryGetComponent<Enemy>(out Enemy en))
                    continue;

            //proboje pobrac component implemetujacy ReciveDamage jesli go znajdzie zadaje obrazenia
            var target = ComponentHelper.GetInterfaceComponent<ReciveDamage>(obj.gameObject);
            if (target != null)
            {
<<<<<<< HEAD
                target.Damage(MStats.AttackDamage, MStats.DamageType, Potion.DamagePlace.Zone, false);
=======
                target.Damage(enemy.MeleeStats.AttackDamage, enemy.MeleeStats.DamageType, Potion.DamagePlace.Zone, false);
>>>>>>> 0b0c13f (naprawa unity i repo)
            }
        }

        // nastepny atak moze byc wykonany dopiero po pewnym czasie
<<<<<<< HEAD
        yield return new WaitForSeconds(MStats.AttackFrequency);
=======
        yield return new WaitForSeconds(enemy.MeleeStats.AttackFrequency);
>>>>>>> 0b0c13f (naprawa unity i repo)
        AttackReady = true;
        yield break;
    }

    
    public IEnumerator ProjectileAttack(Vector2 direction)
    {
        AttackReady = false;

        // Spawnuje obiekt na scenie
<<<<<<< HEAD
        GameObject projectile = Instantiate(PStats.Projectile,transform.position,Quaternion.identity);
=======
        GameObject projectile = Instantiate(enemy.ProjectileStats.Projectile,transform.position,Quaternion.identity);
>>>>>>> 0b0c13f (naprawa unity i repo)

        if (projectile == null)
            yield return -1;

        
        //Ustawia wszystkie komponenty potrzebne do dzia³ania Projectile
        Rigidbody2D rb;
        if(!projectile.TryGetComponent<Rigidbody2D>(out rb))
            rb = projectile.AddComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
<<<<<<< HEAD
        rb.drag = PStats.Drag / 10;
=======
        rb.drag = enemy.ProjectileStats.Drag / 10;
>>>>>>> 0b0c13f (naprawa unity i repo)

        Collider2D collider;
        if(!projectile.TryGetComponent<Collider2D>(out collider))
            collider = projectile.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        SpriteRenderer spriteRenderer;
        if(!projectile.TryGetComponent<SpriteRenderer>(out spriteRenderer))
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 6;

<<<<<<< HEAD
        projectile.AddComponent<Projectile>().stats = PStats;


        // Wystrzeliwuje pocisk w okreœlonym kierunku
        rb.AddForce( direction * PStats.Speed,ForceMode2D.Impulse);


        // Po okreœlonym czasie umo¿liwia wykonanie kolejnego ataku i przerywa coroutine
        yield return new WaitForSeconds(PStats.AttackFrequency);
=======
        projectile.AddComponent<Projectile>().stats = enemy.ProjectileStats;


        // Wystrzeliwuje pocisk w okreœlonym kierunku
        rb.AddForce( direction * enemy.ProjectileStats.Speed,ForceMode2D.Impulse);


        // Po okreœlonym czasie umo¿liwia wykonanie kolejnego ataku i przerywa coroutine
        yield return new WaitForSeconds(enemy.ProjectileStats.AttackFrequency);
>>>>>>> 0b0c13f (naprawa unity i repo)
        AttackReady = true;
        yield break;
    }

}
