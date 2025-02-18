using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PotionEffect))]
public class BeamPotionEffect : MonoBehaviour
{
    [HideInInspector] public Vector2 direction; //ustawiane przez Throw.cs, Throw.cs -> PotionObject.cs lub poprzedni obiekt BeamPotionEffect.cs
    [SerializeField] int length; //ustawiane przez poprzedni obiekt lub jeœli ten jest pierwszy pobierane z potion
    float size;
    bool WasSet;
    Potion potion;

    void Start()
    {
        potion = GetComponent<PotionEffect>().GetPotion();
        size = GetComponent<PotionEffect>().GetSize();
        StartCoroutine(SetLength());
        StartCoroutine(DeployNext());
    }

    IEnumerator DeployNext()
    {
        yield return new WaitForSeconds(potion.BeamSpawnTime);
        if(length > 0)
        {
            // œrodek naszego obiektu plus po³owa jego wielkoœci ¿eby znaleŸæ siê na jego krawêdzi, kierunek, d³ugoœæ promienia - w tym przypadku wielkoœæ naszego obiektu * 0.5
            // potrzebna ¿eby sprawdzic czy jest miejsce na nowy obiekt
            RaycastHit2D hit = Physics2D.Raycast( (Vector2)transform.position + (direction * size * 0.51f) , direction, size * 0.5f);
            if (hit.collider == null || ComponentHelper.GetInterfaceComponent<ReciveDamage>(hit.collider.gameObject) != null || hit.collider.gameObject == gameObject || hit.collider.isTrigger)
            {
                Vector2 position = (Vector2)transform.position + (direction * size);
                BeamPotionEffect newObject = Instantiate(potion.potionEffect, position, transform.rotation).GetComponent<BeamPotionEffect>();
                newObject.direction = direction;
                newObject.SetLength(length - 1);
            }
        }
        yield break;
    }

    public void SetLength(int length)
    {
        this.length = length;
        WasSet = true;
    }
    IEnumerator SetLength()
    {
        yield return new WaitForEndOfFrame();
        if(!WasSet)
        {
            length = potion.BeamLength - 1; //nasz obiekt jest pierwszym obiektem; od ogólnej iloœci obiektów odejmujemy nasz
            transform.position += (Vector3)direction * 0.1f;
        }
        yield break;
    }
}