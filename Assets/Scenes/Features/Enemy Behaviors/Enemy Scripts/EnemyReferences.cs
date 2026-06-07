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
