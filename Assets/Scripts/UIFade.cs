using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    [SerializeField]
    private float fadeTime = 1f;
    [SerializeField]
    private Image fadeImage;

    [SerializeField]
    private GameObject vfxWarmupObject;

    private float alpha = 1f;

    private bool fadeIn = false;
    private bool fadeOut = false;


    private void Start()
    {
        StartCoroutine(VFXWarmupCoroutine());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (fadeIn)
        {
            alpha -= Time.fixedDeltaTime / fadeTime;
            if (alpha <= 0f)
            {
                alpha = 0f;
                fadeIn = false;
            }

            fadeImage.color = new Color(0, 0, 0, alpha);
        }
        else if (fadeOut)
        {
            {
                alpha += Time.fixedDeltaTime / fadeTime;
                if (alpha >= 1f)
                {
                    alpha = 1f;
                    fadeOut = false;
                }

                fadeImage.color = new Color(0, 0, 0, alpha);
            }
        }
    }

    public void Fade(bool fadeIn)
    {
        if (fadeIn == true)
        {
            this.fadeIn = true;
        }
        else
        {
            this.fadeOut = true;
        }
    }

    private IEnumerator VFXWarmupCoroutine()
    {
        yield return new WaitForEndOfFrame();
        Destroy(vfxWarmupObject);
        yield return new WaitForSeconds(0.5f);
        Fade(true);
    }
}
