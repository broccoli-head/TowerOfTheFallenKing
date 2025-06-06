using System;
using System.Collections;
using UnityEditor;
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
    private AudioSource audioSource;
    private AudioClip potionBreakSound;


    void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();
        potion = inventory.FindPotionByName(Name);
        StartCoroutine(DestroyPotion());
        ObjectSize = GetComponent<Collider2D>().bounds.size.x;

        if (potion.shape == Potion.Shape.Beam)
            beamPotionEffect = gameObject.AddComponent<BeamPotionEffect>();

        if (potion.cleanse)
            gameObject.AddComponent<Cleanse>();

        audioSource = GetComponent<AudioSource>();
    }


    void Start()
    {
        potionBreakSound = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().potionBreakSound;

        if (audioSource != null && potion.Name != "Explosive" && CommlinkOpener.checkVisibility())
            audioSource.PlayOneShot(potionBreakSound);


        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ObjectSize/2);
        foreach (Collider2D collider in colliders)
        {
            //On contact damage
            var DmgReciver = Helper.GetInterfaceComponent<ReciveDamage>(collider.gameObject);
            if (DmgReciver != null)
            {
                DmgReciver.Damage(potion.OnContactDamage, potion.damageType, potion.damagePlace, potion.EnemyOnly);
            }

            var controller = Helper.GetInterfaceComponent<Controller>(collider.gameObject);

            //freeze
            if (controller != null && potion.freeze)
            {
                controller.Disable(potion.freezeTime);

                if (DmgReciver != null)
                {
                    OnLeaveDamage Leave = potion.onLeaveDamage;
                    DmgReciver.Damage(Leave.Damage, Leave.Time, potion.damageType, potion.damagePlace, Leave.Effect, potion.EnemyOnly);
                }
            }

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
                if(potion.explosion.Effect != null)
                    StartCoroutine( ExplosionEffect( potion.explosion.EffectTime * 1.5f, Instantiate(potion.explosion.Effect, transform) ));
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

        if (audioSource != null)
        {
            //pu�� d�wi�k potki, je�li menu oraz inventory s� wy��czone
            if (CommlinkOpener.checkVisibility())
            {
                if (!audioSource.isPlaying && audioSource.clip != null)
                    audioSource.Play();
            }
            else audioSource.Stop();
        }
    }

    void FixedUpdate()
    {
        // Normalny damage
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ObjectSize / 2);
        foreach (Collider2D collider in colliders)
        {
            ReciveDamage target = Helper.GetInterfaceComponent<ReciveDamage>(collider.gameObject);
            if (target != null)
            {
                target.Damage(potion.Damage * Time.fixedDeltaTime, potion.damageType, potion.damagePlace, potion.EnemyOnly);
            }

            ReciveSpeedChange speedTarget = Helper.GetInterfaceComponent<ReciveSpeedChange>(collider.gameObject);
            if (potion.speedChange && speedTarget != null)
            {
                speedTarget.ChangeSpeed(potion.speedMultiplier);
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var target = Helper.GetInterfaceComponent<ReciveDamage>(collision.gameObject);
        if (target != null)
        {
            // naklada speedChange - przeciwnik moze poruszac sie szybciej lub wolniej przez pewien czas
            if (potion.speedChange)
            {
                ReciveSpeedChange speedTarget = Helper.GetInterfaceComponent<ReciveSpeedChange>(collision.gameObject);
                speedTarget.ChangeSpeedOnExit(potion.speedTime);
            }
            
            // naklada onLeaveDamage - przeciwnik dostaje obrazenia przez okreslony czas po wyjsciu z obszaru dzialania potki
            if (potion.onLeaveDamage.HaveOnLeaveDamage)
            {
                OnLeaveDamage Leave = potion.onLeaveDamage;
                target.Damage(Leave.Damage, Leave.Time, potion.damageType, potion.damagePlace, Leave.Effect, potion.EnemyOnly);
            }

            // naklada exposedEfect - przeciwnik przez okreslony czas bedzie otrzymywal zwiekszone obrazenia
            if (potion.ExposedEffect)
            {
                target.Expose(potion.ExpositionOverTime);
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


    public float GetSize()
    {
        return ObjectSize;
    }
    public Potion GetPotion()
    {
        return potion;
    }
}