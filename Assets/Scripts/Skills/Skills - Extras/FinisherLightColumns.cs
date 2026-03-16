using UnityEngine;

public class FinisherLightColumns : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayer;

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Debug.Log("Hit");
            other.GetComponentInParent<EnemyHealth>().Damage(35f);
        }
    }
}
