using UnityEngine;

public class LuminousLaserLaser : MonoBehaviour
{
    [SerializeField]
    private float timeToDie = 1.5f;

    private void FixedUpdate()
    {
        timeToDie -= Time.fixedDeltaTime;

        if (timeToDie < 0)
        {
            Destroy(gameObject);
        }
    }
}
