using UnityEngine;

public class LuminousLaserLaser : MonoBehaviour
{
    [SerializeField]
    private float timeToDie = 1.5f;
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private float damage = 50f;
    [SerializeField]
    private float gaugeIncrease = 5f;

    private GaugeManager gaugeManager;

    private void Awake()
    {
        gaugeManager = FindAnyObjectByType<GaugeManager>();
    }

    private void FixedUpdate()
    {
        timeToDie -= Time.fixedDeltaTime;

        if (timeToDie < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Debug.Log("Hit");
            other.GetComponentInParent<EnemyHealth>().Damage(damage);
            gaugeManager.IncreaseGauge(gaugeIncrease, SkillAttunement.Light);
        }
    }
}
