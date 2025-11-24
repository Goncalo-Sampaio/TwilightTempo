using System;
using TMPro;
using UnityEditor;
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
    private NavMeshPath currentPath;
    //Physics 
    private Rigidbody rb;

    [Header("AI Debugging")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 previousTarget;
    [SerializeField] private float chaseTriggerDistance;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float pathUpdateFrequency = 1f;
    [SerializeField] private Renderer _enemyRenderer;
    [SerializeField] private float distanceToTriggerRepath = 1f;
    private Rigidbody playerRb;
    private float groundOffset;
    //private Material _enemyMaterialInstance;
    private Color _defaultEnemyCol;
    private float targetDistance;    
    private bool spottedPlayer = false;
    private bool chasingPlayer = false;
    private float resetPathCalcTimer; 
    
    //UICanvas
    [Header("UI Canvas")]
    [SerializeField] private TextMeshProUGUI _hasPath;
    [SerializeField] private TextMeshProUGUI _playerSpotted;
    [SerializeField] private TextMeshProUGUI _autobreak;
    [SerializeField] private Canvas _canvas;
    public enum EnemyPathState
    {
        Idle,
        Chasing,        
        Attacking,
        Damaged

    }
    private EnemyPathState pathState;

    
    private void OnValidate()
    {
        groundOffset = GetComponent<CapsuleCollider>().height / 2;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        //_enemyMaterialInstance = new Material(_enemyRenderer.material); //Local Instance of material
        //_enemyRenderer.material = _enemyMaterialInstance; //Assign Instance to Renderer
        _defaultEnemyCol = _enemyRenderer.material.color; //store original tint colour

    }
    private void Start()
    {
        Debug.Log($"{enemyName} spawned");
        health = enemyHealth;
        timeTillDestroy = enemyTimeTillDestroy;
        resetPathCalcTimer = pathUpdateFrequency;
        pathState = EnemyPathState.Idle;
        playerRb = target.transform.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        targetDistance = Vector3.Distance(transform.position, target.transform.position);
        //If player within chaseRange:
        if (targetDistance <= chaseTriggerDistance)
        {           
            
            spottedPlayer = true;
            //Sets path if doesn't exist 
            if (!navMeshAgent.hasPath)
            {
                //store current Destination
                previousTarget = target.position; 
                //Set agent's Destination 
                navMeshAgent.SetDestination(target.transform.position);
                //Tell agent to START moving 
                navMeshAgent.isStopped = false;

                Debug.Log("Path has been set");
            }
            //If path already set:
            else
            {
               
                //PATH UPDATING:
                //When timer 0:
                //  Recalculate a new path
                //  Reset timer
                //Note: this can be a couroutine?
                //reminder that previousTarget is the destination we stored the last time we set destination. This won't update until a repath is called.
                
                //Player's offset from previously stored destination
                float targetDelta = Vector3.Distance(target.transform.position, previousTarget);

                //If player moves away from previous destination more then threshold:
                bool targetHasVeered = targetDelta >= distanceToTriggerRepath;
                
                //if timer is 0  OR  player has moved more then threshold
                if (resetPathCalcTimer <= 0 || targetHasVeered)
                {
                    navMeshAgent.SetDestination(target.transform.position); //call for repath
                    resetPathCalcTimer = pathUpdateFrequency; //reset timer
                    previousTarget = target.position; //recache new destination

                }
                //if while being chased the player stops or not:
                if (playerRb.linearVelocity.magnitude >= 0.1f) navMeshAgent.autoBraking = false;
                else navMeshAgent.autoBraking = true;
                
            }
            

        }
        //If player outside chase range:
        else
        {
            //Reset nav
            navMeshAgent.isStopped = true; //Tell agent to STOP moving 
            if (navMeshAgent.hasPath) navMeshAgent.ResetPath(); //clears path if exists
            resetPathCalcTimer = pathUpdateFrequency; //Resets repathing timer

            spottedPlayer = false;
            
        }
        UpdatePathStatus();
        UpdateDebbugStatus();


    }
    private void UpdatePathStatus()
    {
        if (spottedPlayer) pathState = EnemyPathState.Chasing;
        else if (!spottedPlayer) pathState = EnemyPathState.Idle;        
    }
    private void UpdateDebbugStatus()
    {

        UpdateDebbugUI();
        switch (pathState)
        {
            case EnemyPathState.Idle:
                SetMaterialColour(_defaultEnemyCol);
                break;
            case EnemyPathState.Chasing:
                SetMaterialColour(Color.purple);
                break;
            case EnemyPathState.Attacking:
                SetMaterialColour(Color.red);
                break;
            case EnemyPathState.Damaged:
                SetMaterialColour(Color.darkRed);
                break;
        }



    }
    private void UpdateDebbugUI()
    {
        _playerSpotted.text = $"Triggered = " + spottedPlayer;
        _hasPath.text = $"hasPath = " + navMeshAgent.hasPath;
        _autobreak.text = $"Autobreak = " + navMeshAgent.autoBraking;
        _canvas.transform.LookAt(Camera.main.transform.position, Vector3.up);
    }

    private void SetMaterialColour(Color col) => _enemyRenderer.material.color = col;

    protected override void Die()
    {
        base.Die();
        Debug.Log($"{enemyName} died");
    }
    //Debugging:
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Handles.Label(transform.position + Vector3.up * (chaseTriggerDistance + .2f), $"Trigger Distance = {chaseTriggerDistance}");
        Gizmos.DrawWireSphere(transform.position + Vector3.up * -groundOffset, chaseTriggerDistance);
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * -groundOffset, stoppingDistance);
    }

}


