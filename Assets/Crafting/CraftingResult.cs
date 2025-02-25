using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingResult : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    Inventory inventory;
    Image img;
    Color color;
    Item result;
    Crafting crafting;

    void Start()
    {
        inventory = Inventory.Instance;
        img = GetComponent<Image>(); 
        color = img.color;
        crafting = FindAnyObjectByType<Crafting>();
    }

    public void SetPotion(Item result)
    {
        this.result = result;

        if(result != null)
        {
            img.sprite = result.GetSprite();
            img.color = Color.white;
        }
        else
        {
            img.sprite = null;
            img.color = color;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventory.PointedItem = result;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        inventory.PointedItem = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        crafting.ConfirmCrafting();
    }
}
