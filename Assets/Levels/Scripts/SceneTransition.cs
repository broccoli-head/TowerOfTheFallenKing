using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration;
    private AudioSource audioSource;

    public AudioClip menuTransition;
    public AudioClip sceneTransition;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = menuTransition;
        DontDestroyOnLoad(this);
    }

    public IEnumerator SwitchScenes(string sceneName, string comingFrom)
    {
        if (comingFrom == "Default")
            audioSource.PlayOneShot(audioSource.clip);
        else
            audioSource.PlayOneShot(sceneTransition);

        //zmienia ekran na czarny
        yield return StartCoroutine(Fade(0f, 1f));

        //czeka 0.1 sekundy
        yield return new WaitForSeconds(0.1f);

        //³aduje now¹ scenê
        SceneManager.LoadScene(sceneName);

        //rozjaœnia ekran
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }

}