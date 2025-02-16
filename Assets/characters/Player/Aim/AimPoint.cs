using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AimPoint : MonoBehaviour
{
    PotionObject PotionObj;

    void Start()
    {
        StartCoroutine("AutoDestroy");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PotionObject>(out PotionObj))
        {
            PotionObj.transform.position = transform.position;
            PotionObj.deploy();
            Destroy(gameObject);
        }
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
        yield break;
    }
}
