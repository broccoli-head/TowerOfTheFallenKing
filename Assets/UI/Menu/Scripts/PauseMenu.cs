using UnityEngine;

public class PauseMenu : Menu
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject inGameVisible;

    private CommlinkOpener commlinkOpener;
    public static bool isVisible = false;


    void Awake()
    {
        commlinkOpener = FindAnyObjectByType<CommlinkOpener>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !DeathScreen.isVisible)
        {
            if (isVisible)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (CommlinkOpener.IsOpen)
            commlinkOpener.ToggleCommlink();

        inGameVisible.SetActive(false);
        menuUI.SetActive(true);

        //zatrzymuje gre
        Time.timeScale = 0f;
        isVisible = true;
    }

    public void ResumeGame()
    {
        menuUI.SetActive(false);
        inGameVisible.SetActive(true);

        //w³¹cza z powrotem grê
        Time.timeScale = 1f;
        isVisible = false;
    }
}
