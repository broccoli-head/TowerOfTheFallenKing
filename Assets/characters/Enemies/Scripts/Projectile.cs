using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float AttackDamage;
    public Potion.DamageType DamageType;
    public float Speed;
    public float LiveTime;
    public float Drag;

    private void Start()
    {
        StartCoroutine(die());
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ReciveDamage target = Helper.GetInterfaceComponent<ReciveDamage>(other.gameObject);
            target.Damage(AttackDamage,DamageType,Potion.DamagePlace.Zone,false);
            Destroy(this.gameObject);

        }
    }


    protected virtual IEnumerator die()
    {
        yield return new WaitForSeconds(LiveTime);
        Destroy(this.gameObject);
        yield break;
    }

}
