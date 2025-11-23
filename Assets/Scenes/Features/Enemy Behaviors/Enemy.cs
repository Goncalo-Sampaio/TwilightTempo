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
    [SerializeField] private float chaseTriggerDistance;
    [SerializeField] private float pathUpdateFrequency = 1f;
    [SerializeField] private Renderer _enemyRenderer;
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
    [SerializeField] private Canvas _canvas;
    public enum EnemyPathState
    {
        Idle,
        Chasing,        
        Attacking,
        Damaged

    }
    private EnemyPathState pathState;

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
                //Set Destination to player
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
                resetPathCalcTimer -= Time.deltaTime;
                if (resetPathCalcTimer <= 0)
                {
                    navMeshAgent.SetDestination(target.transform.position);
                    resetPathCalcTimer = pathUpdateFrequency;
                    Debug.Log("Path Updated");
                }
                Debug.Log("Agent is running path");
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
        _canvas.transform.LookAt(Camera.main.transform.position, Vector3.up);
    }

    private void SetMaterialColour(Color col) => _enemyRenderer.material.color = col;

    protected override void Die()
    {
        base.Die();
        Debug.Log($"{enemyName} died");
    }
    //Debbugging:

    private void OnDrawGizmos()
    {
        //Trigger distance
        Gizmos.color = Color.red;
        Handles.Label(transform.position + Vector3.up * (chaseTriggerDistance + .2f), $"Trigger = {chaseTriggerDistance}");
        Gizmos.DrawWireSphere(transform.position, chaseTriggerDistance);

    }

}


