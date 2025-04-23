using UnityEngine;

public class PauseMenu : Menu
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject inventory;

    private bool isPaused = false;

    void Update()
    {
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
        inventory.SetActive(false);
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
