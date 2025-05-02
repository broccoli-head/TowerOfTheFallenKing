using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ItemsUI : MonoBehaviour
{

    public ItemUIPrefab prefab;
    [SerializeField] private List<UI> uis;


    void Start()
    {
        RefreshItemsList();
    }

    public void RefreshItemsList()
    {
        // Niszczy wszystkie obiekty itemow w ui
        foreach (UI ui in uis)
        {
            foreach (Transform item in ui.ListUI.transform.GetComponentInChildren<Transform>())
            {
                Destroy(item.gameObject);
            }
        }


        try
        {
            
            foreach (UI ui in uis)
            {
                foreach (var item in Inventory.Instance.PlayerItems)
                {
                    // sprawdzamy czy item istnieje
                    Item i = Inventory.Instance.FindItemByName(item.Name);
                    if (i != null)
                    {
                        // jeœli typ itemu odpowiada typowi listy, tworzy dla niego nowy obiekt w ui
                        if(i.type == ui.ItemType)
                        {
                            GameObject potionUI = Instantiate(prefab.gameObject, ui.ListUI.transform);
                            potionUI.GetComponent<ItemUIPrefab>().SetItem(i);
                        }
                    }
                    else
                    {
                        //Debug.Log($"item: {item.Name} nie istnieje");
                    }

                }
            }

        }
        catch (Exception e)
        {

        }

    }

    [System.Serializable]
    private class UI
    {
        public Item.ItemType ItemType;
        public GameObject ListUI;
    }
}