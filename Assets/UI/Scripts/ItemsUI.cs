using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ItemsUI : MonoBehaviour
{
    Inventory inventory;

    public ItemUIPrefab prefab;

    void Start()
    {
        inventory = GameObject.FindFirstObjectByType<Inventory>().GetComponent<Inventory>();
        RefreshItemsList();
    }

    public void RefreshItemsList()
    {
        foreach (Transform item in transform.GetComponentInChildren<Transform>())
        {
            Destroy(item.gameObject);
        }

        //Potions
        try
        {
            foreach (var item in inventory.PlayerPotions)
            {

                GameObject potionUI = Instantiate(prefab.gameObject, transform);
                Potion potion = inventory.FindPotionByName(item.PotionName);
                potionUI.GetComponent<ItemUIPrefab>().SetItem(potion);
            }

            //Resources
            foreach (var item in inventory.PlayerResources)
            {
                GameObject ResourceUI = Instantiate(prefab.gameObject, transform);
                Resource resource = inventory.FindResourceByName(item.ResourceName);
                ResourceUI.GetComponent<ItemUIPrefab>().SetItem(resource);
            }
        }
        catch (Exception e)
        {

        }

    }
}