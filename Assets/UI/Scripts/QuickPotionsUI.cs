using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickPotionsUI : MonoBehaviour
{
    Inventory inventory;
    GameObject[] slots;


    public static QuickPotionsUI Instance;


    void Start()
    {
        Instance = this;
        inventory = Inventory.Instance;
        slots = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            slots[i] = transform.GetChild(i).gameObject;
        }
    }


    void Update()
    {
        for (int i = 0;i < slots.Length;i++)
        {
            validCheck:
            if(inventory.QuickPotions[i] != null)
            {
                if(inventory.SelectedPotion != null)
                {
                    if (inventory.SelectedPotion.Name == inventory.QuickPotions[i].Name)
                        slots[i].GetComponent<Image>().color = new Color(1,1,1,1);
                    else
                        slots[i].GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f, 0.5f);
                }
                else
                    slots[i].GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f, 0.5f);

                slots[i].GetComponent<Image>().sprite = inventory.QuickPotions[i].sprite;
                try
                {
                    slots[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = inventory.FindPlayerPotionByName(inventory.QuickPotions[i].Name).count.ToString();
                }catch(Exception e)
                {
                    inventory.ValidatePotions();
                    goto validCheck;
                }
                

            }
            else
            {
                slots[i].GetComponent<Image>().sprite = null;
                slots[i].GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.3f);
                slots[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }
}