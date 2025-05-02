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
    UnityEngine.UI.Image image;
    TextMeshProUGUI count;
    [HideInInspector] public Item item;

    private void Awake()
    {
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
        count.font = Inventory.Instance.font;
        count.text = Inventory.Instance.FindPlayerItemByName(item.Name).count.ToString();
        
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    /// Ustawianie Potki wskazywanej przez gracza w UI, uzywane przez ItemDescriptionUI.cs
    public void OnPointerEnter(PointerEventData eventData)
    {
        Inventory.Instance.PointedItem = item;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Inventory.Instance.PointedItem = null;
    }
    /////////////////////////////////////////////////////////////////////////////////////////



    public void OnPointerDown(PointerEventData eventData) 
    {
        // ustawia potke wybrana przez gracza w Inventory.Instance, uzywane do zmiany quick potions przez QuickPotionsSlot.cs i przy craftingu (tbd)
        Inventory.Instance.SelectedItem = item;
        //potka jest resetowana za kazdym otwarciem commlink przez CommlinkOpener.cs
    }

}
