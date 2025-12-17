using UnityEngine;
using UnityEngine.AI;

public class EnemyReferences : MonoBehaviour
{
    //[HideInInspector] public NavMeshAgent navMeshAgent;
    //[SerializeField] public Animator animator; //Activate this when we get an animator   
    [HideInInspector] public EnemyAttack enemeyAttack;
    [HideInInspector] public EnemyNavigation enemyNavigation;
    
    private void Awake()
    {
        enemyNavigation = GetComponent<EnemyNavigation>();
        enemeyAttack = GetComponentInChildren<EnemyAttack>();
        //navMeshAgent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
    }
}
