using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CraftingSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Crafting.ComponentType type;

    Inventory inventory;
    Image img;
    [HideInInspector] public Item item;
    [HideInInspector] public Item.temperature BurningTemperature;
    [HideInInspector] public bool IsSet = false;
    
    void Start()
    {
        inventory = Inventory.Instance;
        img = GetComponent<Image>();
        reset();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (inventory.SelectedItem != null)
        {

            if (!valid(inventory.SelectedItem))
                return;

            reset();

            item = inventory.SelectedItem;

            IsSet = true;

            inventory.RemovePlayerItem(item.Name);
                

            img.sprite = item.GetSprite();
            img.color = Color.white;

            inventory.SelectedItem = null;
            inventory.RefreshPlayerInventory();
            Crafting.Instance.CheckCrafting();
        }
        else
        {
            reset();
            Crafting.Instance.CheckCrafting();
        }
            
    }


    bool valid(Item item)
    {
        if (item.GetStateOfMatter() == Item.StateOfMatter.None)
        {
            Crafting.CraftingError = $"{item.Name} is not suitable for the crafting";
            inventory.SelectedItem = null;
            return false;
        }
        if (type == Crafting.ComponentType.Fuel)
        {
            if (!item.IsFuel)
            {
                Crafting.CraftingError = $"{item.Name} is not suitable for fuel";
                inventory.SelectedItem = null;
                return false;
            }
        }
        else if (type == Crafting.ComponentType.Filling)
        {
            if (item.GetStateOfMatter() != Item.StateOfMatter.Liquid)
            {
                Crafting.CraftingError = $"{item.Name} is not a liquid. Ingredients must be fluid";
                inventory.SelectedItem = null;
                return false;
            }
        }
        else if (type == Crafting.ComponentType.Component)
        {
            if (item.GetStateOfMatter() == Item.StateOfMatter.Liquid)
            {
                Crafting.CraftingError = $"{item.Name} is a liquid. Ingredients can't be fluid.";
                inventory.SelectedItem = null;
                return false;
            }
        }

        return true;
    }


    // Oproznia slot bez zwracania itemu do inventory
    public void EmptySlot()
    {
        item = null;
        reset();
    }
    

    // zwraca item do inventory gracza i opruznia slot
    void reset()
    {
        if (item != null)
        {
            inventory.AddPlayerItem(item.Name);
        }
        inventory.RefreshPlayerInventory();

        IsSet = false;
        img.sprite = null;
        item = null;
        img.color = new Color(1,1,1,0f);
    }

    void Reset()
    {

    }

    ////////////////////////////////////////////////////////////////////////////////////////
    /// Ustawianie Potki wskazywanej przez gracza w UI, uzywane przez ItemDescriptionUI.cs
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
            Inventory.Instance.PointedItem = item;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Inventory.Instance.PointedItem = null;
    }
    /////////////////////////////////////////////////////////////////////////////////////////
}
