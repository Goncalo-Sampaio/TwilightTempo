using UnityEngine;

public class LightDashObject : MonoBehaviour
{
    [SerializeField]
    private float velocity;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float timeToDie = 0.43f;

    private GameObject player;
    private GameObject playerModel;

    private float horizontalInput;

    [SerializeField]
    private LayerMask enemyLayer;

    private GaugeManager gaugeManager;

    private void Awake()
    {
        gaugeManager = FindAnyObjectByType<GaugeManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponentInParent<MovementPlayables>().gameObject;
        playerModel = FindAnyObjectByType<PlayerAnimEventsHandler>().gameObject;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        player.transform.position += playerModel.transform.forward * velocity * Time.fixedDeltaTime;
        playerModel.transform.Rotate(Vector3.up * rotationSpeed * Time.fixedDeltaTime * horizontalInput);

        timeToDie-= Time.fixedDeltaTime;

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
            gaugeManager.IncreaseGauge(10f, SkillAttunement.Light);
        }
    }
}
