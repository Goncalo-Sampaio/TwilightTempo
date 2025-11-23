using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField]private Light myLight;
    [SerializeField]private float maxInterval = 1f;

    private float targetIntensity;
    private float lastIntensity;
    private float interval;
    private float timer;

    [SerializeField] private float maxDisplacement = 0.25f;
    private Vector3 targetPosition;
    private Vector3 lastPosition;
    private Vector3 origin;

    private void Start()
    {
        origin = transform.position;
        lastPosition = origin;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > interval)
        {
            lastIntensity = myLight.intensity;
            targetIntensity = Random.Range(0.5f, 1f);
            timer = 0;
            interval = Random.Range(0, maxInterval);

            targetPosition = origin + Random.insideUnitSphere * maxDisplacement;
            lastPosition = myLight.transform.position;
        }

        myLight.intensity = Mathf.Lerp(lastIntensity, targetIntensity, timer / interval);
        myLight.transform.position = Vector3.Lerp(lastPosition, targetPosition, timer / interval);
    }
}
