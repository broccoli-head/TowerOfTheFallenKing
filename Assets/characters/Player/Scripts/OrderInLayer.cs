using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderInLayer : MonoBehaviour
{
    GameObject player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerLive>().gameObject;
    }

    private void Update()
    {
        transform.position = player.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<SpriteRenderer>(out var spriteRenderer) && collision.gameObject != player)
        {
            if (spriteRenderer.sortingOrder == 4)
            {
                if(collision.transform.position.y < transform.position.y)
                    spriteRenderer.sortingOrder = 5;
                else
                    spriteRenderer.sortingOrder = 3;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<SpriteRenderer>(out var spriteRenderer) && collision.gameObject != player)
        {
            if(spriteRenderer.sortingOrder == 3 || spriteRenderer.sortingOrder == 5)
            {
                spriteRenderer.sortingOrder = 4;
            }
        }
    }
}
