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
    public Item result;
    Crafting crafting;
    [HideInInspector] public int ItemCount;

    void Start()
    {
        inventory = Inventory.Instance;
        img = GetComponent<Image>(); 
        color = img.color;
        crafting = Crafting.Instance;
    }

    public void SetItem(Item result)
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
        //inventory.PointedItem = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        crafting.ConfirmCrafting();
    }
}
