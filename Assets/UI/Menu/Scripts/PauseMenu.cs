using UnityEngine;

public class PauseMenu : Menu
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject inventory;
    CommlinkOpener commlinkOpener;
    private bool isPaused = false;

    void Update()
    {
        commlinkOpener = FindAnyObjectByType<CommlinkOpener>();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        canvas.SetActive(false);

        if (commlinkOpener.IsOpen)
        {
            commlinkOpener.ToggleCommlink();
        }

        
        menuUI.SetActive(true);

        //zatrzymuje gre
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        canvas.SetActive(true);
        menuUI.SetActive(false);

        //trawersujemy po dzieciach canvasa
        foreach (Transform child in canvas.transform)
        {
            //w³¹czamy wszystko, poza inventory
            if (child.gameObject != inventory)
                child.gameObject.SetActive(true);
        }

        //w³¹cza z powrotem grê
        Time.timeScale = 1f;
        isPaused = false;
    }
}
