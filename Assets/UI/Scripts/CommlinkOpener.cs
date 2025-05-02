using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommlinkOpener : MonoBehaviour
{
    [HideInInspector] public bool IsOpen { get; private set; }
    GameObject InGameVisible;
    GameObject InCommlinkVisible;
    Inventory inventory;

    [System.Obsolete]
    void Start()
    {
        IsOpen = false;
        inventory = Inventory.Instance;
        InCommlinkVisible = transform.FindChild("Inventory").gameObject;
        InGameVisible = transform.FindChild("InGameVisible").gameObject;
        InCommlinkVisible.SetActive(IsOpen);
        InGameVisible.SetActive(!IsOpen);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) 
        {
            ToggleCommlink();
        }
        if (IsOpen && Input.GetKeyDown(KeyCode.Tab))
        {
            FindFirstObjectByType<ItemsUI>().RefreshItemsList();
        }
            
    }

    public void ToggleCommlink()
    {
        IsOpen = !IsOpen;

        //resetuje wybrana potke w UI
        inventory.SelectedItem = null;

        if (IsOpen)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
        InCommlinkVisible.SetActive(IsOpen);
        InGameVisible.SetActive(!IsOpen);
    }
}
