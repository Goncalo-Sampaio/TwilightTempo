using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private GameObject hitParticles;
    [SerializeField]
    private LayerMask enemyLayer;

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Debug.Log("Hit");
            Instantiate(hitParticles, transform.position, Quaternion.identity);
        }
    }
}
