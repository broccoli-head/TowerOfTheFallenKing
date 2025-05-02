using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 0.05f;


    public void Start()
    {
        DontDestroyOnLoad(this);
    }

    public IEnumerator SwitchScenes(string sceneName)
    {
        //zmienia ekran na czarny
        yield return StartCoroutine(Fade(0f, 1f));

        //czeka 0.05 sekundy
        yield return new WaitForSeconds(0.05f);

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