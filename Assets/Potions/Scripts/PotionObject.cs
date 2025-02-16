using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PotionObject : MonoBehaviour
{
    [HideInInspector] public Vector2 direction;
    Inventory inventory;
    Potion potion;
    GameObject potionEffect;
    BeamPotionEffect BPE;

    public string Name;

    void Start()
    {
        inventory = GameObject.FindFirstObjectByType<Inventory>();
        potion = inventory.FindPotionByName(Name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            rb.velocity = Vector2.zero;
        deploy();
    }

    public void deploy()
    {
        potionEffect = Instantiate(potion.potionEffect, transform.position, Quaternion.identity);
        if (potionEffect.TryGetComponent<BeamPotionEffect>(out BPE))
        {
            BPE.direction = direction;
        }
        potionEffect.transform.parent = null;
        Destroy(gameObject);
    }
}
