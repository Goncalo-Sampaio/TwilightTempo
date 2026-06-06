using UnityEngine;

public class MusicBombProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private LayerMask enemyLayer;
    //[SerializeField]
    //private float damage = 20f;
    //[SerializeField]
    //private float gaugeIncrease = 15f;

    private bool moving = true;
    private GaugeManager gaugeManager;

    private void Awake()
    {
        gaugeManager = FindAnyObjectByType<GaugeManager>();
        combatData = FindAnyObjectByType<CombatDataManager>().combatData;
    }

    CombatStats combatData;    
    void Start()
    {
        
        Invoke("StopMoving", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    private void StopMoving()
    {
        moving = false;
        Invoke("Explosion", 1);
    }

    private void Explosion()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Debug.Log("Hit");
            Explosion();
            other.GetComponentInParent<EnemyHealth>().Damage(combatData.MusicBomb.Damage, combatData.MusicBomb.KnockbackForce, transform.position);
            gaugeManager.IncreaseGauge(combatData.MusicBomb.GuageIncrease, SkillAttunement.None);
        }
    }
}
