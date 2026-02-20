using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private Color flashColour = Color.white;

    [SerializeField] private int flashIterations = 1;
    [SerializeField] private float maxFlashTime = 3f;
    [SerializeField] private float flashSpeedMultiplier = 20f;
    [SerializeField] private float flashFadeOutSpeed = 5f;
    
    [SerializeField] private float flashCurveMod = 1f;

    [HideInInspector] public bool isFlashing = false;
    
    private Renderer[] renderers;

    private float Lerpthreshold = 0.01f;
    void Start()
    {        
        renderers = GetComponentsInChildren<Renderer>();
    }

    [Button]
    public void FlashForXIterationsBTN() => FlashForXIterations(flashIterations);
    public void FlashForXIterations(int flashes)
    {
        flashIterations = flashes;
        StartCoroutine(FlashMultipleInterationsRot());
    }
    [Button]
    public void FlashForXSecondsBTN() => FlashForXSeconds(maxFlashTime);
    public void FlashForXSeconds(float maxFlashingTime)
    {
        maxFlashTime = maxFlashingTime;
        StartCoroutine(FlashForSecondsRot());
    }

    //Instead of StopCoroutine, we use the isFlashBool.
    //That won't stop the rot right away but it will exit out of the flashing loop and allow for some cleanup
    [Button]
    public void StopFlash()
    {
        if (!isFlashing) return;
        isFlashing = false;
    }
    private IEnumerator FlashForSecondsRot()
    {
        isFlashing = true;
        float startingFlashTimeStamp = Time.time;
        float flashingTimer = Time.time - startingFlashTimeStamp;
        float flashValue = 0f;
        //enable emission
        EnableEmissionInChildren(true);
        yield return null;
        while (isFlashing && maxFlashTime > flashingTimer)
        {
            //  0 - 1 with Power to affect sine curve's 
            float sinInput = flashingTimer * flashSpeedMultiplier;
            flashValue = Mathf.Pow(((Mathf.Sin(sinInput - (Mathf.PI / 2)) + 1) / 2f), flashCurveMod);
            Color flashCol = flashColour * flashValue;
            SetFlashEmissionValueInChildren(flashCol);
            flashingTimer += Time.deltaTime;
            yield return null;
        }

        //Cleanup
        //Fade out flash instead of disabling right away
        while (flashValue > 0f)
        {
            flashValue = Mathf.Clamp01(flashValue);
            Color flashCol = flashColour * flashValue;
            SetFlashEmissionValueInChildren(flashCol);
            flashValue -= Time.deltaTime * flashFadeOutSpeed;
            yield return null;
        }

        EnableEmissionInChildren(false);
        isFlashing = false;
        yield return null;

    }
    
    private IEnumerator FlashMultipleInterationsRot()
    {
        isFlashing = true;
        float startingFlashTimeStamp = Time.time;
        float flashingTimer = Time.time - startingFlashTimeStamp;
        float flashValue = 0f;
        int flashCount = 0;
        float sinInput = 0f;
        EnableEmissionInChildren(true);
        yield return null;
        while (isFlashing && flashIterations > flashCount)
        {
            flashingTimer += Time.deltaTime;
            sinInput = flashingTimer * flashSpeedMultiplier;
            flashCount = Mathf.FloorToInt(sinInput / (2f * Mathf.PI));
            flashValue = Mathf.Pow(((Mathf.Sin(sinInput - (Mathf.PI/2)) + 1) / 2f), flashCurveMod);
            Color flashCol = flashColour * flashValue;
            SetFlashEmissionValueInChildren(flashCol);
            yield return null;
        }
        if (flashValue != 0f)
        {
            flashValue = Mathf.Lerp(flashValue, 0f, flashFadeOutSpeed * Time.deltaTime);            
            if (Mathf.Abs(flashValue) < Lerpthreshold) flashValue = 0f;
            Color flashCol = flashColour * flashValue;
            SetFlashEmissionValueInChildren(flashCol);
            yield return null;
        }
        EnableEmissionInChildren(false);
        isFlashing = false;
        yield return null;
    }
    private void EnableEmissionInChildren(bool on)
    {
        foreach (Renderer renderer in renderers)
        {
            if (on)
            {
                renderer.material.EnableKeyword("_EMISSION");
                //This might mess with bakes:
                renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            else
            {
                renderer.material.DisableKeyword("_EMISSION");
                renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
        }
    }
    private void SetFlashEmissionValueInChildren(Color flashColour)
    {
        foreach (Renderer renderer in renderers)renderer.material.SetColor("_EmissionColor", flashColour);
    }


    
    
    

}

