using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private GameObject hitParticles;
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private GaugeManager gaugeManager;

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Debug.Log("Hit");
            other.GetComponentInParent<EnemyHealth>().Damage(35f);
            Instantiate(hitParticles, transform.position, Quaternion.identity);
            gaugeManager.IncreaseGauge(10f, SkillAttunement.None);
        }
    }
}
