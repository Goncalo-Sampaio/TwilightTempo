using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("UI Elements")]
    public Image titleImage;              // Title image
    public TextMeshProUGUI pressText;     // "Press any key" text

    [Header("Settings")]
    public float fadeDuration = 1f;       // Duration of fade in/out

    private bool canPress = false;

    void Start()
    {
        // Start with alpha 0
        SetAlpha(titleImage, 0f);
        SetAlpha(pressText, 0f);

        // Fade in the title, then start the text blinking
        StartCoroutine(FadeInTitleAndText());
    }

    void Update()
    {
        if (canPress && Input.anyKeyDown)
        {
            SceneManager.LoadScene("MainMenu"); // Load MainMenu scene
        }
    }

    private IEnumerator FadeInTitleAndText()
    {
        // Fade in the title
        yield return StartCoroutine(FadeGraphic(titleImage, 0f, 1f, fadeDuration));

        // Start blinking the text
        canPress = true;
        StartCoroutine(BlinkText(pressText, fadeDuration));
    }

    // Make the text blink in a loop
    private IEnumerator BlinkText(TextMeshProUGUI text, float duration)
    {
        while (true)
        {
            // Fade in
            yield return StartCoroutine(FadeGraphic(text, 0f, 1f, duration));
            // Fade out
            yield return StartCoroutine(FadeGraphic(text, 1f, 0f, duration));
        }
    }

    // Fade for any Graphic (Image or TextMeshProUGUI)
    private IEnumerator FadeGraphic(Graphic graphic, float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            SetAlpha(graphic, alpha);
            yield return null;
        }
        SetAlpha(graphic, endAlpha);
    }

    // Helper to set alpha
    private void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic != null)
        {
            Color c = graphic.color;
            c.a = alpha;
            graphic.color = c;
        }
    }
}

