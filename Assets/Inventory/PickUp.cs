using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public string ItemName;

    float size = 0.5f;
    Inventory inventory;
    SpriteRenderer spriteRenderer;
    Collider2D Collider;
    Item item;


    void Start()
    {
        inventory = Inventory.Instance;
        item = inventory.FindItemByName(ItemName);

        if (item == null)
        {
            Debug.Log(ItemName + " is not existing item!");
            Destroy(gameObject);
            return;
        }

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 2;
        spriteRenderer.sprite = item.GetSprite();
        Collider = gameObject.AddComponent<CircleCollider2D>();
        Collider.isTrigger = true;

        Vector3 originalSize = spriteRenderer.bounds.size;
        transform.localScale = new Vector3(
            size / Mathf.Max(originalSize.x, 0.01f),
            size / Mathf.Max(originalSize.y, 0.01f)
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.CompareTag("Player"))
        {
            inventory.AddPlayerItem(item);
            Destroy(gameObject);
        }
    }


    //zwraca obiekt ze skryptem PickUp ktory mozna umiescic na scenie
    public static GameObject Object(string ItemName)
    {
        GameObject obj = new GameObject(ItemName);
        PickUp Comp = obj.AddComponent<PickUp>();
        Comp.ItemName = ItemName;
        return obj;
    }
}
