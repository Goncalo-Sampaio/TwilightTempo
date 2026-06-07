using UnityEngine;
using UnityEngine.AI;

public class EnemyReferences : MonoBehaviour
{    
    [HideInInspector] public EnemyAttack enemeyAttack;
    [HideInInspector] public EnemyCasterAttack enemyCasterAttack;
    [HideInInspector] public EnemyNavigation enemyNavigation;
    [HideInInspector] public EnemyHealth enemyHealth;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public EnemyAnimator enemyAnimator;
    [HideInInspector] public Flash flash;
    [HideInInspector] public EnemyBrain enemyBrain;
    [HideInInspector] public ParticleSystem berserkParticles;
    [HideInInspector] public UIManager uIManager;
    [HideInInspector] public EnemySoundManager enemySoundManager;
    public WaypointHandler WayPoints;
    //temporary:
    [HideInInspector] public bool isCaster = false;
    public EnemyScriptableObject enemyData;
    public Transform playerRef;
    
    private void Awake()
    {
        //Set something by default so this doesn't just crap itself from no reference:
        playerRef = FindAnyObjectByType<PlayerHealth>().transform;
        if (WayPoints == null)
        {
            //if no waypoint object is present then just create one and add this transform as its only waypoint
            WayPoints = gameObject.AddComponent<WaypointHandler>();
            WayPoints.wayPoints.Add(gameObject.transform);
            Debug.Log($"Warning {gameObject.name} did not have an assigned Waypoint Handler, created single patroll point on object instead");
        }
        enemyBrain = GetComponent<EnemyBrain>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyNavigation = GetComponent<EnemyNavigation>();
        enemeyAttack = GetComponentInChildren<EnemyAttack>();
        enemyCasterAttack = GetComponentInChildren<EnemyCasterAttack>();
        rb = GetComponent<Rigidbody>();
        enemyAnimator = GetComponentInChildren<EnemyAnimator>();
        flash = GetComponentInChildren<Flash>();
        berserkParticles = GetComponentInChildren<ParticleSystem>();
        uIManager = FindAnyObjectByType<UIManager>();
        enemySoundManager = GetComponent<EnemySoundManager>();
    }
    private void Start()
    {
        if (enemyData.enemyType != EnemyType.Caster) isCaster = false;
        else isCaster = true;
    }

}
