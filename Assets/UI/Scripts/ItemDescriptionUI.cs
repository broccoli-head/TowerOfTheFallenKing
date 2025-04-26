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
            tmp.text += inventory.PointedItem.Description;

            if (inventory.PointedItem.GetAttributes().Count > 0) {
                tmp.text += "<br>→ Attributes:";
                foreach(var attr in inventory.PointedItem.GetAttributes())
                {
                    tmp.text += $"<br>  ● {attr.attribute.ToString()} ({attr.value})";
                }
            }

            if (inventory.PointedItem.IsFuel)
            {
                tmp.text += $"<br>→ Combustion temperature: {inventory.PointedItem.BurningTemperature.ToString()}";
            }
            
        }
        else
        {
            tmp.text = "";
        }
    }
}
