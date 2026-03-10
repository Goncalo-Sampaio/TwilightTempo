using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBrain : MonoBehaviour
{
    //Handles high level logic. and calls subcomponent methods    
    //Pools data from other subcomponents to execute desicions
    [HideInInspector]public bool engaged = false;
    
    [Header("CHASE params")]    

    [SerializeField] private float chaseUpdateFrequency = 0.2f;

    private bool playerInsideTrigger = false;

    [Header("COMBAT params")]
    [SerializeField] private float attackUpdateFrequency = 0.2f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRangeTolerance = .3f;
    [Tooltip("How the attack's collider is active for:")]
    [SerializeField] private float attackWindow = 1f;
    
    //References
    private EnemyReferences enemyReferences;

    //STATEMACHINE CLASS:
    private StateMachine stateMachine;

    //Local vars
    private bool gettingKnockBacked = false; //set from EnemyHealth
    private float triggerColliderRadius;
    private float groundOffset;
    public bool wasHit;
    public bool dead;
    public string state;

    private bool playerInsideTriggerRadius, playerWithinLineOfSight, withinAttackRange;
    private bool playerWasSpoted;

    [SerializeField] private float forgetTimmer = 5f;
    private float forgetTimmerCountdown;


    private void Awake()
    {
        playerWasSpoted = false;
        stateMachine = new StateMachine();
        enemyReferences = GetComponent<EnemyReferences>();
    }


    private void Start()
    {
        //Set the riggidbody to kinematic | set gravity to null on start
        enemyReferences.rb.useGravity = false;
        enemyReferences.rb.isKinematic = true;

        enemyReferences.enemeyAttack.SetAttackWindow(attackWindow);
        groundOffset = GetComponentInChildren<CapsuleCollider>().height / 2;
        
        
        //STATES
        var idle = new EnemyState_Idle(enemyReferences);
        var chase = new EnemyState_Chase(enemyReferences, chaseUpdateFrequency);
        var combat = new EnemyState_Combat(enemyReferences, attackUpdateFrequency);
        var gotHit = new EnemyState_GotHit(enemyReferences);
        var death = new EnemyState_Death();
        //var delay = new EnemyState_Delay(2f);

        //TRANSITIONS
        At(idle, chase, () => engaged); //This will update inside an enumerator. Ideally checked inside a fixedUpdate
        At(chase, idle, () => !engaged);
        
        At(combat, chase, () => engaged && !withinAttackRange); //Outside attack range but still within line of sight
        //(delay,() => enemyReferences.enemyHealth.dead);
        //Make a knockedback STATE
        //Any(delay, () => gettingKnockBacked);
        Any(gotHit, () => wasHit);
        Any(death, () => dead);
        Any(combat, () => withinAttackRange && engaged);
        //Transition from got hit to the rest of the states:
        At(gotHit, chase, ()=> !wasHit && PlayerDetected() && !dead);
        At(gotHit, combat, () => !wasHit && CloseEnoughToAttack() && !dead);
        //At(delay, chase, () => PlayerDetected() );
        //START STATE


        stateMachine.SetState(idle);
        //delay needs exit condition


        //FUNCTIONS & CONDITIONS
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to,Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
    }
    
    private bool PlayerDetected()
    {        
        if (playerInsideTrigger) return enemyReferences.enemyNavigation.HasLineOfSight(enemyReferences.playerRef.position);
        else return false;
    }
    private bool CloseEnoughToAttack()
    {
        float distanceToPlayer = enemyReferences.enemyNavigation.LinearDistanceFromTarget(enemyReferences.playerRef.position);

        bool isInsideRange = distanceToPlayer < attackRange + attackRangeTolerance && distanceToPlayer > attackRange - attackRangeTolerance;
        Debug.Log($"Min = {attackRange - attackRangeTolerance} Current = {distanceToPlayer}, Max = {attackRange + attackRangeTolerance}, Is inside range = {isInsideRange}, engaged = {engaged} ");

        return isInsideRange;
    }

    //player detection
    private void ProbeSurroundings ()
    {
        playerInsideTrigger = enemyReferences.enemyNavigation.PlayerInsideTriggerDistance();
        if (playerInsideTrigger || engaged )
        {
            //Only probe line of sight if:
            //  Player is inside sphere trigger
            //  This Enemy is activly engaged with the player (Meaning it spotted them and is either chasing or attacking the player)
            //reminder that the check is only done here:
            playerWithinLineOfSight = enemyReferences.enemyNavigation.HasLineOfSight(enemyReferences.playerRef.position, "Player");
            if (playerWithinLineOfSight )
            {
                //Enemy spots the player
                engaged = true;
                forgetTimmerCountdown = forgetTimmer;
            }
        }
        if(engaged && !playerWithinLineOfSight)
        {
            forgetTimmerCountdown -= Time.deltaTime;
            if(forgetTimmerCountdown <= 0f) engaged = false;
        }     
        
        

        withinAttackRange = enemyReferences.enemyNavigation.LinearDistanceFromTarget(enemyReferences.playerRef.position) <= attackRange;
    }
    

    private void Update()
    {
        stateMachine.Tick();
    }
    private void FixedUpdate()
    {
        ProbeSurroundings();
    }
    
    //private void ShortTermMemory()
    //{
        
    //    //Reset timer
    //    if (PlayerDetected())
    //    {
    //        engaged = true;
    //        forgetTimmerCountdown = forgetTimmer;
    //    }
    //    else
    //    {
    //        forgetTimmerCountdown -= Time.deltaTime;
    //    }
    //    if (forgetTimmerCountdown <= 0f) engaged = false;
    //    //Debug.Log($"Forget me timer: {forgetTimmerCountdown}");

    //}

    private void OnDrawGizmos()
    {
        if (stateMachine != null)
        {
            Gizmos.color = stateMachine.GetGizmoColor();
            Gizmos.DrawSphere(transform.position + Vector3.up * 3, 0.4f);
        }
    }

    





}
