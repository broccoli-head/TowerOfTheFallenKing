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

        // Ustawiane dla pewno�ci �e nie b�dzie r�wne niczemu innemu ni� null dopuki gracz niczego nie wybierze 
        inventory.SelectedItem = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(inventory.SelectedItem != null && inventory.SelectedItem.IsPotion())  // sprawdzi� czy wybrany item to potion!
        {
            for(int i = 0;i < inventory.QuickPotions.Length;i++)
            {
                // Je�li w Quick Potions znajduje si� ju� potka kt�r� chcemy tam umie�ci�
                // - przesuwamy na jej miejsce potk� z pola na kt�re klikn�� gracz
                // - ustawiamy j� na miejscu wybranym przez gracza
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
