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

        // Ustawiane dla pewnosci ze nie bedzie rowne niczemu innemu niz null dopuki gracz niczego nie wybierze 
        inventory.SelectedItem = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(inventory.SelectedItem != null && inventory.SelectedItem.IsPotion())  // sprawdzic czy wybrany item to potion!
        {
            for(int i = 0;i < inventory.QuickPotions.Length;i++)
            {
                // Jesli w Quick Potions znajduje sie juz potka ktora chcemy tam umiescic
                // - przesuwamy na jej miejsce potke z pola na ktore kliknal gracz
                // - ustawiamy ja na miejscu wybranym przez gracza
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
