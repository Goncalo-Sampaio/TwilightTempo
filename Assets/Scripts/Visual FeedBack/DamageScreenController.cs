using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class DamageScreenController : MonoBehaviour
{
    [SerializeField]private CinemachineImpulseSource _impulseSource;
    [SerializeField]private Material screenDamageMat;
    [SerializeField] private float impactLerpSpeed = 8f;//a good value
    private Coroutine screenDamageRoutine;
    [Header("Vignette Radius")]
    [SerializeField] private Vector2 fromMinFromMax = new Vector2(.85f,1f);
    [SerializeField] private Vector2 toMinToMax = new Vector2(0.4f, -0.15f);
    

    //Make this a static singleton so other objects can call the DamageEffect easier

    private void Update()
    {
        //if (Input.GetMouseButtonDown(1)) ScreenDamageEffect(Random.Range(.1f, 1f));
    }
    public void ScreenDamageEffect(float intensity, Vector3 force)
    {

    }
    public void ScreenDamageEffect(float intensity)
    {
        if(screenDamageRoutine != null)
        {
            StopCoroutine(screenDamageRoutine);
        }
        Vector3 force = new Vector3(0f, -0.5f, -1f);
        screenDamageRoutine = StartCoroutine(ScreenDamageRot(intensity, force));
    }
    private IEnumerator ScreenDamageRot(float intensity,Vector3 force)
    {
        //Camera Shake:
        force.Normalize();
        _impulseSource.GenerateImpulse(force * intensity * 0.4f);

        //Screen Effect:
        float targetRadius = Remap(intensity, fromMinFromMax.x, fromMinFromMax.y, toMinToMax.x, toMinToMax.y);
        float currentRadius = 1; //No damage
        
        for(float t = 0;currentRadius != targetRadius ;t += Time.deltaTime * impactLerpSpeed)
        {
            currentRadius = Mathf.Lerp(1, targetRadius, t);
            screenDamageMat.SetFloat("_Vignette_Radius", currentRadius);
            yield return null;
        }
        for (float t = 0f; currentRadius < 1f; t += Time.deltaTime )
        {
            currentRadius = Mathf.Lerp(targetRadius, 1, t);
            //currentRadius = Mathf.Lerp(targetRadius, 1f, Mathf.SmoothStep(0.0f, 1.0f, t));
            screenDamageMat.SetFloat("_Vignette_Radius", currentRadius);
            yield return null;
        }

    }
    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
    }
}
