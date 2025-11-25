using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
[DefaultExecutionOrder(1)]
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
    

    //Attacking
    [SerializeField] private LayerMask attackMask;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackCoolDown = 2f;
    [SerializeField] private float attackrange = 2f ;
    [SerializeField] private float sphereCastRadius = 1f;
    [SerializeField] private RaycastHit hit;
    private float attackStart;
    private bool attacking;
    private Vector3 debbugAttackDir;
    public bool overrideNav;

    [SerializeField] private Animator attackAnim;

    //Swap to local variable later when the timming is figured out:
    //private WaitForSeconds attackWaitForSeconds;
    //Here you add the player health reference:


    //UICanvas
    [Header("UI Canvas")]
    [SerializeField] private TextMeshProUGUI _hasPath;
    [SerializeField] private TextMeshProUGUI _playerSpotted;
    [SerializeField] private TextMeshProUGUI _autobreak;
    [SerializeField] private Canvas _canvas;
    public enum EnemyState
    {
        Idle,
        Chasing,        
        Attacking,
        Damaged

    }
    private EnemyState DefaultState;
    private EnemyState state;
    public EnemyState State
    {
        get { return state; }
        set 
        {
            OnStateChange?.Invoke(state, value);
            state = value; }
    }
    public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
    public StateChangeEvent OnStateChange;

    
    private void OnValidate()
    {
        groundOffset = GetComponent<CapsuleCollider>().height / 2;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        _defaultEnemyCol = _enemyRenderer.material.color;        
        FightManager.Instance.Enemies.Add(this); //Register this Enemy in fight manager. This should happen dinamicly and not at awake

    }
    private void Start()
    {
        Debug.Log($"{enemyName} spawned");
        health = enemyHealth;
        timeTillDestroy = enemyTimeTillDestroy;
        resetPathCalcTimer = pathUpdateFrequency;
        state = EnemyState.Idle;
        attacking = false;
        overrideNav = false;
        playerRb = target.transform.GetComponent<Rigidbody>();
    }
    private void Update()
    {
     
        if (!overrideNav)
        {targetDistance = Vector3.Distance(transform.position, target.transform.position);
            //If player within chaseRange:
            if (targetDistance <= chaseTriggerDistance)
            {
                if (targetDistance <= attackrange)
                {
                    attacking = true;

                    //If has line of sight :
                    Debug.Log($"Has line of sight = {HasLineOfSight(target)}");
                    if (HasLineOfSight(target))
                    {
                        //Disable NavMeshAgent
                        navMeshAgent.enabled = false;
                        transform.LookAt(target);
                        //If has line of sight + not on cooldown
                        if (Time.time >= attackStart + attackCoolDown)
                        {
                            //If has line of sight + not on cooldown
                            attackStart = Time.time;
                            //ATTACK
                            attackAnim.SetTrigger("Attack");
                            Debug.Log("Player attacked");
                        }
                    }
                    //if no line of sight:
                    else
                    {
                        //If player is within attack range but not in line of sight:
                        navMeshAgent.enabled = true;
                        //THIS CAN JUST DEFAULT BACK TO CHASING TBH:
                        bool successfull = navMeshAgent.SetDestination(target.transform.position);
                        //MOVE around obstacle
                        Debug.Log($"Pathing success = {successfull}");
                        Debug.Log("Need to move closer");

                    }
                }

                //If not close enough then chase

                //CHASING:
                else
                {
                    navMeshAgent.enabled = true;
                    attacking = false;
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
        }
        
        UpdatePathStatus();
        UpdateDebbugStatus();


    }

    public void MoveTo(Vector3 position)
    {
        overrideNav = true;
        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(position);
    }
    
    private bool HasLineOfSight (Transform target)
    {
        Vector3 TargetHitPos = new Vector3(target.position.x, attackPoint.position.y, target.position.z);
        Vector3 attackDir = (TargetHitPos - attackPoint.position).normalized;
        debbugAttackDir = attackDir;
        if (Physics.SphereCast(attackPoint.position, sphereCastRadius, attackDir,out hit, attackrange,attackMask))
        {
            return hit.collider.GetComponent<PlayerAnimEventsHandler>() != null;
        }
        return false;
    }

    private void UpdatePathStatus()
    {
        if (spottedPlayer && !attacking) state = EnemyState.Chasing;
        else if (!spottedPlayer && !attacking) state = EnemyState.Idle;
        else if (spottedPlayer && attacking) state = EnemyState.Attacking;
    }
    private void UpdateDebbugStatus()
    {

        UpdateDebbugUI();
        switch (state)
        {
            case EnemyState.Idle:
                SetMaterialColour(_defaultEnemyCol);
                break;
            case EnemyState.Chasing:
                SetMaterialColour(Color.purple);
                break;
            case EnemyState.Attacking:
                SetMaterialColour(Color.blue);
                break;
            case EnemyState.Damaged:
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
        //Handles.Label(transform.position + Vector3.up * (chaseTriggerDistance + .2f), $"Trigger Distance = {chaseTriggerDistance}");
        Gizmos.DrawWireSphere(transform.position + Vector3.up * -groundOffset, chaseTriggerDistance);
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * -groundOffset, stoppingDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * -groundOffset, attackrange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(attackPoint.position, attackPoint.position + debbugAttackDir * 2);
    }

}


