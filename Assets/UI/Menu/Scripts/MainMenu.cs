using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        LevelLoader.Instance.StartGame();
    }
}
