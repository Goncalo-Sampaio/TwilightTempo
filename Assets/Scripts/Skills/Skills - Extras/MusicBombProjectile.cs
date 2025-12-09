using UnityEngine;

public class MusicBombProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private LayerMask enemyLayer;

    private bool moving = true;
    private GaugeManager gaugeManager;

    private void Awake()
    {
        gaugeManager = FindAnyObjectByType<GaugeManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("StopMoving", 1);
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
            gaugeManager.IncreaseGauge(10f, SkillAttunement.None);
        }
    }
}
