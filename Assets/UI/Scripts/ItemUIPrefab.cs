using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ItemUIPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    Inventory inventory;
    UnityEngine.UI.Image image;
    TextMeshProUGUI count;
    [HideInInspector] public Item item;

    private void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();
        foreach(var item in GetComponentsInChildren<UnityEngine.UI.Image>())
        {
            if (item.gameObject.name == "Image")
                image = item;
        }
        foreach(var item in GetComponentsInChildren<TextMeshProUGUI>()) 
        {
            if (item.gameObject.name == "Count")
                count = item;
        }
    }

    public void SetItem(Item item)
    {
        this.item = item;
        image.sprite = item.GetSprite();
        if (item.IsPotion())
        {
            var name = ((Potion)item).Name;
            count.text = inventory.FindPlayerPotionByName(name).count.ToString();
        }
        else
        {
            var name = ((Resource)item).Name;
            count.text = inventory.FindPlayerResourceByName(name).count.ToString();
        }
        
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    /// Ustawianie Potki wskazywanej przez gracza w UI, u¿ywane przez ItemDescriptionUI.cs
    public void OnPointerEnter(PointerEventData eventData)
    {
        inventory.PointedItem = item;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        inventory.PointedItem = null;
    }
    /////////////////////////////////////////////////////////////////////////////////////////



    public void OnPointerDown(PointerEventData eventData) 
    {
        // ustawia potkê wybrana przez gracza w inventory, u¿ywane do zmiany quick potions przez QuickPotionsSlot.cs i przy craftingu (tbd)
        inventory.SelectedItem = item;
        //potka jest resetowana za ka¿dym otwarciem commlink przez CommlinkOpener.cs
    }

}
