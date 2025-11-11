using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class Enemy : Entity
{
    [Header("Enemy Properties")]
    [SerializeField] private string enemyName;
    [SerializeField] private float enemyHealth;
    [SerializeField] private float enemyDamage;
    [SerializeField] private float enemyTimeTillDestroy;

    //Navigation 
    private NavMeshAgent navMeshAgent;

    //Physics 
    private Rigidbody rb;

    [Header("AI Debugging")]
    [SerializeField] private Transform target;
    [SerializeField] private float chaseTriggerDistance;
    private float targetDistance;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Debug.Log($"{enemyName} spawned");
        health = enemyHealth;
        timeTillDestroy = enemyTimeTillDestroy;
    }
    private void Update()
    {
        targetDistance = Vector3.Distance(transform.position, target.transform.position);
        if (targetDistance <= chaseTriggerDistance)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = target.transform.position;
        }
        else
        {
            navMeshAgent.isStopped = true;
        }
        Debug.Log(navMeshAgent.isStopped);
    }
    protected override void Die()
    {
        base.Die();
        Debug.Log($"{enemyName} died");
    }
}

