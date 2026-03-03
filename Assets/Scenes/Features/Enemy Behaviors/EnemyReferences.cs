using UnityEngine;
using UnityEngine.AI;

public class EnemyReferences : MonoBehaviour
{
    //[HideInInspector] public NavMeshAgent navMeshAgent;
    //[SerializeField] public Animator animator; //Activate this when we get an animator   
    [HideInInspector] public EnemyAttack enemeyAttack;
    [HideInInspector] public EnemyNavigation enemyNavigation;
    [HideInInspector] public EnemyHealth enemyHealth;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public EnemyAnimator enemyAnimator;
    [HideInInspector] public Flash flash;
    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyNavigation = GetComponent<EnemyNavigation>();
        enemeyAttack = GetComponentInChildren<EnemyAttack>();
        rb = GetComponentInChildren<Rigidbody>();
        enemyAnimator = GetComponentInChildren<EnemyAnimator>();
        flash = GetComponentInChildren<Flash>();
        //navMeshAgent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
    }
}
