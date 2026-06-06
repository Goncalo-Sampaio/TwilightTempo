using UnityEngine;

public class FinisherLightColumns : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayer;
    //[SerializeField]
    //private float damage = 100f;
    CombatStats combatData;
    private void Awake()
    {
        combatData = FindAnyObjectByType<CombatDataManager>().combatData;
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Debug.Log("Hit");
            other.GetComponentInParent<EnemyHealth>().Damage(combatData.Finisher.Damage);
        }
    }
}
