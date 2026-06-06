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
    [SerializeField] private GameObject DeflectedProjectile;
    [SerializeField] private Transform playerDir;
    private void Awake()
    {
        
        combatData = FindAnyObjectByType<CombatDataManager>().combatData;
    }
    //lets test something;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            Debug.Log("Deflected!");
            Destroy(other.gameObject);
            Instantiate(hitParticles, transform.position, Quaternion.identity);
            Vector3 deflectDirFlat = new Vector3(playerDir.forward.x, 0, playerDir.forward.z).normalized;
            Instantiate(DeflectedProjectile, transform.parent.position, Quaternion.LookRotation(deflectDirFlat));
        }
        if ((enemyLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Debug.Log("Hit");
            other.GetComponentInParent<EnemyHealth>().Damage(combatData.BaseAttack.Damage, combatData.BaseAttack.KnockbackForce);
            Instantiate(hitParticles, transform.position, Quaternion.identity);
            gaugeManager.IncreaseGauge(combatData.BaseAttack.GuageIncrease, SkillAttunement.None);
        }
        
    }
}
