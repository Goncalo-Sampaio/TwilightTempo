using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private GameObject hitParticles;
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private GaugeManager gaugeManager;
    //[SerializeField]
    //private float damage = 10f;
    //[SerializeField]
    //private float gaugeIncrease = 5f;
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
            other.GetComponentInParent<EnemyHealth>().Damage(combatData.BaseAttack.Damage, combatData.BaseAttack.KnockbackForce);
            Instantiate(hitParticles, transform.position, Quaternion.identity);
            gaugeManager.IncreaseGauge(combatData.BaseAttack.GuageIncrease, SkillAttunement.None);
        }
    }
}
