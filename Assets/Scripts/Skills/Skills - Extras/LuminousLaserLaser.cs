using UnityEngine;

public class LuminousLaserLaser : MonoBehaviour
{
    [SerializeField]
    private float timeToDie = 1.5f;
    [SerializeField]
    private LayerMask enemyLayer;

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
            other.GetComponentInParent<EnemyHealth>().Damage(35f);
            gaugeManager.IncreaseGauge(10f, SkillAttunement.Light);
        }
    }
}
