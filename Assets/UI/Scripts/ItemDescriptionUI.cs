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
        inventory = Inventory.Instance;
    }
 
    void Update()
    {
        if(inventory.PointedItem != null)
        {
            tmp.text = inventory.PointedItem.Name + "<br>";
            tmp.text += inventory.PointedItem.Description + "<br>";


            //if (inventory.PointedItem.GetAttributes().Count > 0)
            //{
            //    tmp.text += "→ Attributes:<br>";
            //    foreach (var attr in inventory.PointedItem.GetAttributes())
            //    {
            //        tmp.text += $"  ● {attr.attribute.ToString()} ({attr.value}) <br>";
            //    }
            //}

            //if (inventory.PointedItem.IsFuel)
            //{
            //    tmp.text += "Item can be used as fuel<br>";
            //    tmp.text += $"→ Combustion temperature: {inventory.PointedItem.BurningTemperature.ToString()} <br>";
            //}

        }
        else
        {
            tmp.text = "";
        }
    }
}
