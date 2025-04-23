using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ItemsUI : MonoBehaviour
{
    Inventory inventory;

    public ItemUIPrefab prefab;
    public GameObject PotionListUI;
    public GameObject ResourceListUI;


    void Start()
    {
        inventory = Inventory.Instance;
        RefreshItemsList();
    }

    public void RefreshItemsList()
    {
        // Niszczy wszystkie obiekty potek i zasobow w ui
        foreach (Transform item in PotionListUI.transform.GetComponentInChildren<Transform>())
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in ResourceListUI.transform.GetComponentInChildren<Transform>())
        {
            Destroy(item.gameObject);
        }


        try
        {
            // Tworzy nowe obiekty potek w ui
            foreach (var item in inventory.PlayerPotions)
            {

                GameObject potionUI = Instantiate(prefab.gameObject, PotionListUI.transform);
                Potion potion = inventory.FindPotionByName(item.PotionName);
                potionUI.GetComponent<ItemUIPrefab>().SetItem(potion);
            }

            // Tworzy nowe obiekty zasobow w ui
            foreach (var item in inventory.PlayerResources)
            {
                GameObject ResourceUI = Instantiate(prefab.gameObject, ResourceListUI.transform);
                Resource resource = inventory.FindResourceByName(item.ResourceName);
                ResourceUI.GetComponent<ItemUIPrefab>().SetItem(resource);
            }
        }
        catch (Exception e)
        {

        }

    }
}