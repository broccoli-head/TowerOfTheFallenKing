using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemDescriptionUI : MonoBehaviour
{
    TextMeshProUGUI tmp;
    Inventory inventory;

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        inventory = FindObjectOfType<Inventory>();
    }
 
    void Update()
    {
        if(inventory.PointedItem != null)
        {
            tmp.text = inventory.PointedItem.GetDescription();
        }
        else
        {
            tmp.text = "";
        }
    }
}
