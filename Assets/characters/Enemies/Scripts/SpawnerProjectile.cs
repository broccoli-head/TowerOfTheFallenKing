using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnerProjectile : Projectile
{
    public GameObject SpawnObject;
    public GameObject HitSpawnObject;


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ReciveDamage target = Helper.GetInterfaceComponent<ReciveDamage>(other.gameObject);
            target.Damage(AttackDamage, DamageType, Potion.DamagePlace.Zone, false);

            if(HitSpawnObject != null)
                Instantiate(HitSpawnObject, transform.position, Quaternion.identity);

            Destroy(this.gameObject);

        }
        // Projectile niszczy sie tez na scianach
        else if (other.TryGetComponent( typeof(TilemapCollider2D),out var tile ))
        {
            if (SpawnObject != null)
                Instantiate(SpawnObject, transform.position, Quaternion.identity);

            Destroy(this.gameObject);
        }
    }

    protected override IEnumerator die()
    {
        yield return new WaitForSeconds(LiveTime);

        if(SpawnObject != null)
            Instantiate(SpawnObject,transform.position, Quaternion.identity);

        Destroy(this.gameObject);
        yield break;
    }
}
