using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    public TextMeshProUGUI deathTitle;
    public TextMeshProUGUI statistics;
    public GameObject background;
    public GameObject inGameVisible;
    private string fullText;
    
    [Space(10)]
    public float typingSpeed;
    public Image flashImage;
    public float flashDuration;
    
    [Space(10)]
    public CinemachineVirtualCamera virtualCam;
    public float targetZoom;
    public float zoomDuration;

    public static bool isVisible = false;
    public static int killsCount = 0;
    private static int deathCount = 0;


    void Start()
    {
        Time.timeScale = 0f;
        isVisible = true;
        fullText = deathTitle.text;
        deathCount++;
        StartCoroutine(Zoom());
    }

    private IEnumerator TypeText()
    {
        deathTitle.gameObject.SetActive(true);
        deathTitle.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            char c = fullText[i];
            deathTitle.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        StartCoroutine(Flash());
    }


    private IEnumerator Flash()
    {
        //wejscie
        float t = 0f;
        while (t < flashDuration / 2)
        {
            float alpha = Mathf.Lerp(0, 1, t / (flashDuration / 2));
            flashImage.color = new Color(1, 0, 0, alpha);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        inGameVisible.SetActive(false);

        statistics.text = "Kills: " + killsCount.ToString() + "\nDeaths: " + deathCount.ToString();
        statistics.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        //wyjscie
        t = 0f;
        while (t < flashDuration / 2)
        {
            float alpha = Mathf.Lerp(1, 0, t / (flashDuration / 2));
            flashImage.color = new Color(1, 0, 0, alpha);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        flashImage.color = new Color(1, 0, 0, 0);
    }


    private IEnumerator Zoom()
    {
        yield return null;  //czeka jedną klatke zeby najpierw wypozycjonować kamere a dopiero pozniej przyzoomować
        float time = 0f;
        float startZoom = virtualCam.m_Lens.OrthographicSize;

        while (time < zoomDuration)
        {
            virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, targetZoom, time / zoomDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        virtualCam.m_Lens.OrthographicSize = targetZoom;
        StartCoroutine(TypeText());
    }


    public void ContinueGame()
    {
        killsCount = 0;
        Time.timeScale = 1f;
        LevelLoader.Instance.StartGame();
        isVisible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
