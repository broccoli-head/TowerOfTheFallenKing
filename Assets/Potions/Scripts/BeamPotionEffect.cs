using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PotionEffect))]
public class BeamPotionEffect : MonoBehaviour
{
    [HideInInspector] public Vector2 direction; //ustawiane przez Throw.cs, Throw.cs -> PotionObject.cs lub poprzedni obiekt BeamPotionEffect.cs
    [SerializeField] int length; //ustawiane przez poprzedni obiekt lub jesli ten jest pierwszy pobierane z potion
    float size;
    bool WasSet;
    Potion potion;


    void Start()
    {
        potion = GetComponent<PotionEffect>().GetPotion();
        size = GetComponent<PotionEffect>().GetSize();
        StartCoroutine(SetLength());
        StartCoroutine(DeployNext());


        if (TryGetComponent<Animator>(out Animator anim))
        {
            if (Mathf.Abs(direction.normalized.x) > 0.5f)
                anim.SetBool("Side", true);
            if (direction.normalized.x < 0)
                GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    IEnumerator DeployNext()
    {
        yield return new WaitForSeconds(potion.BeamSpawnTime);
        if(length > 0)
        {
            // srodek naszego obiektu plus polowa jego wielkosci zeby znalezc sie na jego krawedzi, kierunek, dlugosc promienia - w tym przypadku wielkosc naszego obiektu * 0.5
            // potrzebna zeby sprawdzic czy jest miejsce na nowy obiekt
            RaycastHit2D hit = Physics2D.Raycast( (Vector2)transform.position + (direction * size * 0.51f) , direction, size * 0.5f);
            if (hit.collider == null || Helper.GetInterfaceComponent<ReciveDamage>(hit.collider.gameObject) != null || hit.collider.gameObject == gameObject || hit.collider.isTrigger)
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
            length = potion.BeamLength - 1; //nasz obiekt jest pierwszym obiektem; od ogolnej ilosci obiektow odejmujemy nasz
            transform.position += (Vector3)direction * 0.1f;
        }
        yield break;
    }
}