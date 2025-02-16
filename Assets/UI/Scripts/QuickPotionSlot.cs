using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickPotionSlot : MonoBehaviour, IPointerDownHandler
{
    Inventory inventory;

    public int Index;

    void Start()
    {
        inventory = FindFirstObjectByType<Inventory>();

        // Ustawiane dla pewnoœci ¿e nie bêdzie równe niczemu innemu ni¿ null dopuki gracz niczego nie wybierze 
        inventory.SelectedItem = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(inventory.SelectedItem != null && inventory.SelectedItem.IsPotion())  // sprawdziæ czy wybrany item to potion!
        {
            for(int i = 0;i < inventory.QuickPotions.Length;i++)
            {
                // Jeœli w Quick Potions znajduje siê ju¿ potka któr¹ chcemy tam umieœciæ
                // - przesuwamy na jej miejsce potkê z pola na które klikn¹³ gracz
                // - ustawiamy j¹ na miejscu wybranym przez gracza
                // powoduje to zamiane miejscami potek
                if (inventory.QuickPotions[i] == inventory.SelectedItem)
                {
                    inventory.QuickPotions[i] = inventory.QuickPotions[Index];
                    break;
                }
            }
            inventory.QuickPotions[Index] = (Potion) inventory.SelectedItem; 
            inventory.SelectedItem = null;
        }   
    }
}
