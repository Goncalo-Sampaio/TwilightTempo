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

    [SerializeField]
    private GameObject forestCam;
    [SerializeField]
    private float titleFadeDuration = 2f;
    [SerializeField]
    private float buttonsAppearDelay = 2f;
    [SerializeField]
    private float buttonsInteractableDelay = 3f;
    [SerializeField]
    private TextMeshProUGUI[] buttonsText;
    private float buttonsTimeCounter = 0f;
    private bool buttonsTimeCounting = false;
    private bool buttonsFadeStarted = false;

    private bool waitingForInput = true;
    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    void Start()
    {
        buttonsTimeCounter = buttonsInteractableDelay;
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
            canPress = false;
            //SceneManager.LoadScene("MainMenu"); // Load MainMenu scene
            forestCam.SetActive(true);
            waitingForInput = false;
            StartCoroutine(FadeGraphic(titleImage, 1f, 0f, fadeDuration));
            buttonsTimeCounting = true;
        }
    }

    private void FixedUpdate()
    {
        if (buttonsTimeCounting)
        {
            buttonsTimeCounter -= Time.fixedDeltaTime;

            if (buttonsTimeCounter <= buttonsInteractableDelay - buttonsAppearDelay && !buttonsFadeStarted)
            {
                buttonsFadeStarted = true;
                foreach (TextMeshProUGUI button in buttonsText)
                {
                    StartCoroutine(FadeGraphic(button, 0f, 1f, buttonsTimeCounter));
                }
            }

            if (buttonsTimeCounter <= 0)
            {
                foreach (TextMeshProUGUI button in buttonsText)
                {

                    button.GetComponentInParent<Button>().interactable = true;
                }

                buttonsTimeCounting = false;
            }
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
        while (waitingForInput)
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

