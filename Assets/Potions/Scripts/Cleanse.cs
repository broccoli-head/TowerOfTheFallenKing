using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleanse : MonoBehaviour
{
    void Update()
    {
        foreach(var item in Physics2D.OverlapCircleAll(transform.position, GetComponent<Collider2D>().bounds.size.x))
        {
            var CleanseRevicer = ComponentHelper.GetInterfaceComponent<ReciveDamage>(item.gameObject);
            if (CleanseRevicer != null)
                CleanseRevicer.AddCleanse();
            item.TryGetComponent<PotionEffect>(out var potionEffect);
            if (potionEffect != null && potionEffect.gameObject != this.gameObject)
                Destroy(potionEffect.gameObject);
        }
    }
}
