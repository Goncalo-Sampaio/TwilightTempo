using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LaserBurnDecal : MonoBehaviour
{
    [SerializeField]
    private float timeToDisappear;
    [SerializeField]
    private float delay;

    private DecalProjector decalProjector;
    private float fadeFactor = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        decalProjector = GetComponent<DecalProjector>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        delay -= Time.fixedDeltaTime;

        if (delay <= 0f)
        {
            fadeFactor = decalProjector.fadeFactor - Time.fixedDeltaTime / timeToDisappear;

            if (fadeFactor <= 0f)
            {
                fadeFactor = 0f;
            }

            decalProjector.fadeFactor = fadeFactor;
        }
    }
}
