using NaughtyAttributes;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Flash : MonoBehaviour
{
    
    [SerializeField] private Color flashColour = Color.white;
    [SerializeField] private float maxFlashTime = 3f;
    [SerializeField] private float flashFrequency = 20f;
    [SerializeField] private float flashFadeOutSpeed = 5f;
      

    [SerializeField] private float flashCurveMod = 2f;

    [HideInInspector] public bool isFlashing = false;
    
    private Renderer[] renderers;

    void Start()
    {        
        renderers = GetComponentsInChildren<Renderer>();
    }
    private IEnumerator FlashRot()
    {
        isFlashing = true;
        float startingFlashTimeStamp = Time.time;
        //always output starting at 0 from timestamp
        float flashingTimer = Time.time - startingFlashTimeStamp;
        float flashValue = 0f;
        //enable emission
        EnableEmissionInChildren(true);
        yield return null;
        //Stop flash either by flippin isFlashing to false or by timer
        while (isFlashing && maxFlashTime > flashingTimer)
        {
            //  0 - 1 with Power to affect sine curve's 
            flashValue = Mathf.Pow(((Mathf.Sin(flashingTimer * flashFrequency) +1 )/2f), flashCurveMod);            
            Color flashCol = flashColour * flashValue;
            SetFlashEmissionValueInChildren(flashCol);
            flashingTimer += Time.deltaTime;
            yield return null;
        }

        //Cleanup
        //Fade out flash instead of disabling right away
        while(flashValue > 0f)
        {
            Color flashCol = flashColour * flashValue;
            SetFlashEmissionValueInChildren(flashCol);
            flashValue -= Time.deltaTime * flashFadeOutSpeed;
            yield return null;
        }

        EnableEmissionInChildren(false);
        isFlashing = false;
        yield return null;

    }
    [Button]
    public void StartFlash() => StartCoroutine(FlashRot());
    //Instead of StopCoroutine, we use the isFlashBool.
    //That won't stop the rot right away but it will exit out of the flashing loop and allow for some cleanup
    [Button]
    public void StopFlash()
    {
        if (!isFlashing) return;
        isFlashing = false;
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

