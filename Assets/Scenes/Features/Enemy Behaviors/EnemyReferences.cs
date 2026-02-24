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
    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyNavigation = GetComponent<EnemyNavigation>();
        enemeyAttack = GetComponentInChildren<EnemyAttack>();
        rb = GetComponentInChildren<Rigidbody>();
        //navMeshAgent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
    }
}
