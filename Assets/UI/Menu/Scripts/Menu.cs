using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    [SerializeField] private AudioClip clickSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Button[] buttons = FindObjectsOfType<Button>(true);

        //dla kazdego przycisku ustawiamy listenera
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                //puszcza dŸwiêk klikniêcia
                audioSource.PlayOneShot(clickSound);
            });
        }
    }

    public void StartGame()
    {
        LevelLoader.Instance.StartGame();
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
