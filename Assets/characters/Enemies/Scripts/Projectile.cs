using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public AttackHandler.ProjectileStats stats;

    private void Start()
    {
        StartCoroutine(die());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ReciveDamage target = ComponentHelper.GetInterfaceComponent<ReciveDamage>(other.gameObject);
            target.Damage(stats.AttackDamage,stats.DamageType,Potion.DamagePlace.Zone,false);
            Destroy(this.gameObject);

        }
    }

    IEnumerator die()
    {
        yield return new WaitForSeconds(stats.LiveTime);
        Destroy(this.gameObject);
        yield break;
    }

}
