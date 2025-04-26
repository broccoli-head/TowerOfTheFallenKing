using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ItemsUI : MonoBehaviour
{

    public ItemUIPrefab prefab;
    public GameObject PotionListUI;
    public GameObject ResourceListUI;


    void Start()
    {
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
            foreach (var item in Inventory.Instance.PlayerPotions)
            {
                GameObject potionUI = Instantiate(prefab.gameObject, PotionListUI.transform);
                Potion potion = Inventory.Instance.FindPotionByName(item.PotionName);
                potionUI.GetComponent<ItemUIPrefab>().SetItem(potion);
            }

            // Tworzy nowe obiekty zasobow w ui
            foreach (var item in Inventory.Instance.PlayerResources)
            {
                GameObject ResourceUI = Instantiate(prefab.gameObject, ResourceListUI.transform);
                Resource resource = Inventory.Instance.FindResourceByName(item.ResourceName);
                if(resource != null)
                    ResourceUI.GetComponent<ItemUIPrefab>().SetItem(resource);
            }
        }
        catch (Exception e)
        {

        }

    }
}