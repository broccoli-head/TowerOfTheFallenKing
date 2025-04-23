using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CraftingSlot : MonoBehaviour, IPointerDownHandler
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
            IsSet = true;
            item = inventory.SelectedItem;

            if (item.IsPotion())
            {
                inventory.RemovePlayerPotion(item.GetName());
                BurningTemperature = Item.temperature.Normal;
            }
            else
            {
                inventory.RemovePlayerResource(item.GetName());
                BurningTemperature = ((Resource)item).BurningTemperature;
            }
                

            img.sprite = item.GetSprite();
            img.color = Color.white;

            inventory.SelectedItem = null;
            inventory.RefreshPlayerInventory();
        }
        else
            reset();
    }

    // Oproznia slot bez zwracania itemu do inventory
    public void EmptySlot()
    {
        item = null;
        reset();
    }
    

    void reset()
    {
        if (item != null)
        {
            if(item.IsPotion())
            {
                inventory.AddPlayerPotion(item.GetName());
            }else
                inventory.AddPlayerResource(item.GetName());
        }
        inventory.RefreshPlayerInventory();

        IsSet = false;
        img.sprite = null;
        item = null;
        BurningTemperature = Item.temperature.Normal;
        img.color = new Color(1,1,1,0f);
    }

    void Reset()
    {

    }
}
