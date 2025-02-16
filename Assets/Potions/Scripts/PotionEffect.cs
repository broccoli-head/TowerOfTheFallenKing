using System;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider2D))]

public class PotionEffect : MonoBehaviour
{
    public string Name;

    private Potion potion;
    private Inventory inventory;
    private BeamPotionEffect beamPotionEffect;
    private float ObjectSize;

    private void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();
        potion = inventory.FindPotionByName(Name);
        StartCoroutine(DestroyPotion());
        ObjectSize = GetComponent<Collider2D>().bounds.size.x;
        if (potion.shape == Potion.Shape.Beam)
            beamPotionEffect = gameObject.AddComponent<BeamPotionEffect>();
        if (potion.cleanse)
        {
            //transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
            gameObject.AddComponent<Cleanse>();
        }
    }
    void Start()
    {   
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ObjectSize/2);
        foreach (Collider2D collider in colliders)
        {
            //On contact damage
            var Implementing = ComponentHelper.GetInterfaceComponent<ReviceDamage>(collider.gameObject);
            if (Implementing != null)
            {
                Implementing.Damage(potion.OnContactDamage, potion.damageType, potion.damagePlace, potion.EnemyOnly);
            }

            var controller = ComponentHelper.GetInterfaceComponent<Controller>(collider.gameObject);

            //freeze
            if(controller != null && potion.freeze)
                controller.Disable(potion.freezeTime);

            //Explosion
            Rigidbody2D rb;
            if (collider.gameObject.TryGetComponent<Rigidbody2D>(out rb) && potion.explosion.IsExplosive)
            {        
                if (controller != null)
                    controller.Disable(0.5f);
                Vector2 direction;
                if (potion.shape != Potion.Shape.Beam)
                    direction = (rb.position - (Vector2)transform.position).normalized;
                else
                    direction = beamPotionEffect.direction;
                rb.AddForce(direction * potion.explosion.Force, ForceMode2D.Impulse);
                StartCoroutine( ExplosionEffect( potion.explosion.EffectTime, Instantiate(potion.explosion.Effect, transform) ));
            }
        }
    }

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ObjectSize / 2);
        foreach (Collider2D collider in colliders)
        {
            //Interaction
            if (collider.gameObject.TryGetComponent<PotionEffect>(out PotionEffect other))
            {
                Component interaction = inventory.Interact(Name, other.Name);
                if (interaction != null)
                    gameObject.AddComponent(interaction.GetType());
            }
        }
    }

    void FixedUpdate()
    {
        // Normalny damage
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ObjectSize / 2);
        foreach (Collider2D collider in colliders)
        {
            ReviceDamage target = ComponentHelper.GetInterfaceComponent<ReviceDamage>(collider.gameObject);
            if (target != null)
            {
                target.Damage(potion.Damage * Time.fixedDeltaTime, potion.damageType, potion.damagePlace, potion.EnemyOnly);
            }

            ReviceSpeedChange speedTarget = ComponentHelper.GetInterfaceComponent<ReviceSpeedChange>(collider.gameObject);
            if (potion.speedChange && speedTarget != null)
            {
                speedTarget.ChangeSpeed(potion.speedMultiplier);
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var target = ComponentHelper.GetInterfaceComponent<ReviceDamage>(collision.gameObject);
        if (target != null)
        {
            if (potion.speedChange)
            {
                ReviceSpeedChange speedTarget = ComponentHelper.GetInterfaceComponent<ReviceSpeedChange>(collision.gameObject);
                speedTarget.ChangeSpeedOnExit(potion.speedTime);
            }
            
            if (potion.onLeaveDamage.HaveOnLeaveDamage)
            {
                OnLeaveDamage Leave = potion.onLeaveDamage;
                target.Damage(Leave.Damage, Leave.Time, potion.damageType, potion.damagePlace, Leave.Effect, potion.EnemyOnly);
            }

            if (potion.onLeaveDamage.HaveOnLeaveDamage && potion.exposedEffect)
            {
                OnLeaveDamage Leave = potion.onLeaveDamage;
                ReviceSpeedChange speedTarget = ComponentHelper.GetInterfaceComponent<ReviceSpeedChange>(collision.gameObject);
                float exposed1dmg = potion.Damage * (1 + potion.exposed1Percentage / 100);
                float exposed2dmg = potion.Damage * (1 + potion.exposed2Percentage / 100);

                //np. jesli exposed1dur wynosi 3s, a exposed2dur 10s to bedzie tak to wygladalo:
                //stage 1: classic dmg (np. przez 3 sek)
                target.Damage(
                    Leave.Damage, potion.exposed1Duration, potion.damageType,
                    potion.damagePlace, Leave.Effect, potion.EnemyOnly
                );

                //stage 2:  (np. dmg +10% przez 10 sek)
                StartCoroutine(IncreaseDamage(
                    exposed1dmg, potion.exposed1Duration,
                    potion.exposed2Duration, Leave, target, speedTarget
                ));

                //stage 3:  (np. dmg +25% i koniec efektu)
                StartCoroutine(IncreaseDamage(
                    exposed2dmg, potion.exposed2Duration, 0,
                    Leave, target, speedTarget
                ));
            }
        }
    }

    private IEnumerator DestroyPotion()
    {
        if(potion.Time > 1.1f)
        {
            yield return new WaitForSeconds(potion.Time - 1f);
            // Play animation
            yield return new WaitForSeconds(1);
        }
        else 
            yield return new WaitForSeconds(potion.Time);
        Destroy(gameObject);
    }

    private IEnumerator ExplosionEffect(float time,GameObject effect)
    {
        yield return new WaitForSeconds(time);
        Destroy(effect);
        yield break;
    }

    private IEnumerator IncreaseDamage(float increase, float delay, float time, OnLeaveDamage Leave, ReviceDamage target, ReviceSpeedChange speedTarget)
    {
        yield return new WaitForSeconds(delay);

        if (time == 0)
        {
            target.Damage(increase, potion.damageType, potion.damagePlace, potion.EnemyOnly);
            speedTarget.ChangeSpeed(potion.speedMultiplier, potion.speedTime);
        }
        else
        {
            target.Damage(increase, time, potion.damageType, potion.damagePlace, Leave.Effect, potion.EnemyOnly);
        }
        
    }


    public float GetSize()
    {
        return ObjectSize;
    }
    public Potion GetPotion()
    {
        return potion;
    }
}