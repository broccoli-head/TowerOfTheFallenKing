using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommlinkOpener : MonoBehaviour
{
    bool isOpen = false;
    GameObject InGameVisible;
    GameObject InCommlinkVisible;
    Inventory inventory;

    [System.Obsolete]
    void Start()
    {
        inventory = FindFirstObjectByType<Inventory>();
        InCommlinkVisible = transform.FindChild("InCommlinkVisible").gameObject;
        InGameVisible = transform.FindChild("InGameVisible").gameObject;
        InCommlinkVisible.SetActive(isOpen);
        InGameVisible.SetActive(!isOpen);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) 
        {
            isOpen = !isOpen;

            //resetuje wybrana potke w UI
            inventory.SelectedItem = null;
        
            if(isOpen)
                Time.timeScale = 0f;
            else 
                Time.timeScale = 1f;
            InCommlinkVisible.SetActive(isOpen);
            InGameVisible.SetActive(!isOpen);
        }
        if (isOpen && Input.GetKeyDown(KeyCode.Tab))
        {
            FindFirstObjectByType<ItemsUI>().RefreshItemsList();
        }
            
    }
}
