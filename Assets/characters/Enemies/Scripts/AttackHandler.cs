using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class AttackHandler : MonoBehaviour
{

    Enemy enemy;
    bool AttackReady = true;

    [System.Serializable]
    public struct MeleeStats 
    {
        public bool MeleAttack;
        public float AttackDamage;
        public Potion.DamageType DamageType;
        public float AttackFrequency;
        public float AttackDistance;
        [ReadOnly] public bool InRange;
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
        AttackReady = true;
    }

    void Update()
    {
        if (enemy.IsAgresive && AttackReady)
        {

            bool attacked = false;
            if(enemy.MeleeStats.MeleAttack && enemy.MeleeStats.InRange)
            {
                StartCoroutine(MeleeAttack());
                attacked = true;
            }
            else if (enemy.ProjectileStats.ProjectileAttack)
            {
                Vector2 direction = (enemy.Player.transform.position - transform.position).normalized;
                StartCoroutine(ProjectileAttack(direction));
                attacked = true;
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
;
    }


    public IEnumerator MeleeAttack()
    {

        AttackReady = false;
        //opoznienie ataku
        yield return new WaitForSeconds(enemy.MeleeStats.AttackDelay);

        //zadawanie obrazen
        Collider2D[] Attacked = Physics2D.OverlapCircleAll(transform.position, enemy.MeleeStats.AttackDistance + (GetComponent<Collider2D>().bounds.size.y / 2));
        foreach (Collider2D obj in Attacked)
        {
            //przeciwnik nie atakuje samego siebie
            if (obj.gameObject == gameObject) continue;

            // jesli frendly fire jest wylaczony pomija innych przeciwnikow
            if (!enemy.MeleeStats.FriendlyFire)
                if (obj.TryGetComponent<Enemy>(out Enemy en))
                    continue;

            //proboje pobrac component implemetujacy ReciveDamage jesli go znajdzie zadaje obrazenia
            var target = ComponentHelper.GetInterfaceComponent<ReciveDamage>(obj.gameObject);
            if (target != null)
            {
                target.Damage(enemy.MeleeStats.AttackDamage, enemy.MeleeStats.DamageType, Potion.DamagePlace.Zone, false);
            }
        }

        // nastepny atak moze byc wykonany dopiero po pewnym czasie
        yield return new WaitForSeconds(enemy.MeleeStats.AttackFrequency);
        AttackReady = true;
        yield break;
    }

    
    public IEnumerator ProjectileAttack(Vector2 direction)
    {
        AttackReady = false;

        // Spawnuje obiekt na scenie
        GameObject projectile = Instantiate(enemy.ProjectileStats.Projectile,transform.position,Quaternion.identity);

        if (projectile == null)
            yield return -1;

        
        //Ustawia wszystkie komponenty potrzebne do dzia³ania Projectile
        Rigidbody2D rb;
        if(!projectile.TryGetComponent<Rigidbody2D>(out rb))
            rb = projectile.AddComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.drag = enemy.ProjectileStats.Drag / 10;

        Collider2D collider;
        if(!projectile.TryGetComponent<Collider2D>(out collider))
            collider = projectile.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        SpriteRenderer spriteRenderer;
        if(!projectile.TryGetComponent<SpriteRenderer>(out spriteRenderer))
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 6;

        projectile.AddComponent<Projectile>().stats = enemy.ProjectileStats;


        // Wystrzeliwuje pocisk w okreœlonym kierunku
        rb.AddForce( direction * enemy.ProjectileStats.Speed,ForceMode2D.Impulse);


        // Po okreœlonym czasie umo¿liwia wykonanie kolejnego ataku i przerywa coroutine
        yield return new WaitForSeconds(enemy.ProjectileStats.AttackFrequency);
        AttackReady = true;
        yield break;
    }

}
