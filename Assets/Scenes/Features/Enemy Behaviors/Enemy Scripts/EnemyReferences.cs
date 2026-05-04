using UnityEngine;
using UnityEngine.AI;

public class EnemyReferences : MonoBehaviour
{    
    [HideInInspector] public EnemyAttack enemeyAttack;
    [HideInInspector] public EnemyNavigation enemyNavigation;
    [HideInInspector] public EnemyHealth enemyHealth;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public EnemyAnimator enemyAnimator;
    [HideInInspector] public Flash flash;
    [HideInInspector] public EnemyBrain enemyBrain;
    [HideInInspector] public ParticleSystem berserkParticles;
    //temporary:
    public bool isCaster = false;
    public EnemyCasterAttack enemyCasterAttack;
    public Transform playerRef;
    private void Awake()
    {

        enemyBrain = GetComponent<EnemyBrain>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyNavigation = GetComponent<EnemyNavigation>();
        enemeyAttack = GetComponentInChildren<EnemyAttack>();
        enemyCasterAttack = GetComponentInChildren<EnemyCasterAttack>();
        rb = GetComponent<Rigidbody>();
        enemyAnimator = GetComponentInChildren<EnemyAnimator>();
        flash = GetComponentInChildren<Flash>();
        berserkParticles = GetComponentInChildren<ParticleSystem>();
    }
}
