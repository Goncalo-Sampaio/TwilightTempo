using DG.Tweening;
using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class EnemyBrain : MonoBehaviour
{
    //Handles high level logic. and calls subcomponent methods    
    //Pools data from other subcomponents to execute desicions
    
    
    [Header("CHASE params")]
    [SerializeField] private float chaseUpdateFrequency = 0.2f;

    private bool playerInsideTrigger = false;

    [Header("COMBAT params")]
    [SerializeField] private float attackUpdateFrequency = 0.2f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRangeTolerance = .3f;
    [Tooltip("How the attack's collider is active for:")]
    [SerializeField] private float attackWindow = 1f;
    [SerializeField] private float staggerTimmer = .75f;

    //References
    private EnemyReferences enemyReferences;

    //STATEMACHINE CLASS:
    private StateMachine stateMachine;

    //Local vars
    private bool gettingKnockBacked = false; //set from EnemyHealth
    
    private float groundOffset;
    [HideInInspector] public bool wasHit;
    [HideInInspector] public bool dead;
    [HideInInspector] public bool isBerserk;
    private float defaultArrivalDistance;

    private bool playerWithinLineOfSight, withinAttackRange;
    private bool playerFirstSpoted;
    public bool engaged = false;    
    [SerializeField] private float forgetTimmer = 5f;
    private float forgetTimmerCountdown;
    private Collider[] colliders;

    [SerializeField] private VisualEffect burst;
    [SerializeField] private ParticleSystem shockwave;
    private void Awake()
    {
        defaultStaggerTime = staggerTimmer;
        playerFirstSpoted = true;
        stateMachine = new StateMachine();
        enemyReferences = GetComponent<EnemyReferences>();
        colliders = GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
        defaultArrivalDistance = enemyReferences.enemyNavigation.agent.stoppingDistance;
        groundOffset = GetComponentInChildren<CapsuleCollider>().height / 2;
        forgetTimmerCountdown = forgetTimmer;
        //STATES
        var idle = new EnemyState_Idle(enemyReferences, patrolLingerTime);
        var chase = new EnemyState_Chase(enemyReferences, chaseUpdateFrequency);
        var combat = new EnemyState_Combat(enemyReferences, attackUpdateFrequency);
        var gotHit = new EnemyState_GotHit(enemyReferences);
        var death = new EnemyState_Death();
        var berserk = new EnemyState_Berserk(enemyReferences);
        //TRANSITIONS
        At(idle, chase, () => engaged && !dead ); 
        At(chase, idle, () => !engaged && !enteringBerserkState && !dead && !isBerserk);        
        At(combat, chase, () => engaged && !dead && (!withinAttackRange  || !playerWithinLineOfSight) );  
        At(gotHit, chase, ()=> !wasHit && engaged && !dead);
        At(gotHit, combat, () => !wasHit && withinAttackRange && engaged && !dead && playerWithinLineOfSight );        
        At(berserk, chase, () => !enteringBerserkState && engaged && !dead );
        At(berserk, combat, () => !enteringBerserkState && !wasHit && withinAttackRange && engaged && !dead);

        Any(gotHit, () => wasHit && !dead && !enteringBerserkState );
        Any(death, () => dead);
        Any(combat, () => withinAttackRange && engaged && !dead && !enteringBerserkState && playerWithinLineOfSight);
        Any(berserk, () => enteringBerserkState && !dead);

        //START STATE
        //should be called inside the iddle sttate
        stateMachine.SetState(idle);

        //FUNCTIONS & CONDITIONS
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to,Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);

    }
   
    
    private void Update()
    {
        stateMachine.Tick();

    }
    
    private void FixedUpdate()
    {
        //I also need to :
        //when line of sight is broke just go to the last place the player was spotted and stop.
        ProbeSurroundings();
        if (engaged && !dead)
        {
            if (playerFirstSpoted)
            {
                enemyReferences.enemySoundManager.PlaySpottedPlayerSoundEffect();
                playerFirstSpoted = false;
            }
            //Alert Nearby enemies:
            AlertNearbyEnemies();
        }


    }
    private Coroutine patrolRoutine;
    private int currentPatrolPointIndex = 0;
    [SerializeField] private float patrolLingerTime = 6f;
    [SerializeField] private float patrolLingerTimeVariation = 2f;
    public bool isPatrolling  = false;
    
    public void StartPatrol()
    {
        //Change arrival distance to closer one
       enemyReferences.enemyNavigation.agent.stoppingDistance = 1f;
        //if there's only one waypoint
        if (enemyReferences.enemyNavigation.HasArrivedAtTarget() && enemyReferences.WayPoints.wayPoints.Count == 1) return;
        if (!isPatrolling) patrolRoutine = StartCoroutine(PatrolRoutine());        
    }
    
    public void StopPatrol()
    {
        //change arrival distance back
        enemyReferences.enemyNavigation.agent.stoppingDistance = defaultArrivalDistance;
        if (patrolRoutine != null) StopCoroutine(patrolRoutine);
        enemyReferences.enemyNavigation.moving = false;
        enemyReferences.enemyNavigation.StopNow(true);

        enemyReferences.enemyAnimator.StopWalking();
        
        isPatrolling = false;

    }    
    private IEnumerator PatrolRoutine()
    {
        currentPatrolPointIndex = enemyReferences.WayPoints.wayPoints.IndexOf(enemyReferences.WayPoints.GetClosestWaypoint(transform.position));

        while (true)
        {
            isPatrolling = true;

            if (currentPatrolPointIndex >= enemyReferences.WayPoints.wayPoints.Count) currentPatrolPointIndex = 0;
            
            enemyReferences.enemyNavigation.StopNow(false);
            enemyReferences.enemyNavigation.moving = true;

            
            yield return null;          
            enemyReferences.enemyNavigation.MoveTo(enemyReferences.WayPoints.wayPoints[currentPatrolPointIndex].position);

            
            enemyReferences.enemyAnimator.StopIdle();
            yield return null; //pause before switching
            enemyReferences.enemyAnimator.StartWalking();

            yield return new WaitForEndOfFrame();
            
            yield return new WaitForFixedUpdate();

            
            yield return new WaitUntil(() => enemyReferences.enemyNavigation.HasArrivedAtTarget());


            enemyReferences.enemyNavigation.moving = false;
            enemyReferences.enemyNavigation.StopNow(true);

            enemyReferences.enemyAnimator.StopWalking();
            yield return null;
            enemyReferences.enemyAnimator.StartIdle();

            float lingerTime = patrolLingerTime + UnityEngine.Random.Range(-patrolLingerTimeVariation, patrolLingerTimeVariation);
            yield return new WaitForSeconds(lingerTime);

           
            currentPatrolPointIndex++;
        }
    }
    
    private void AlertNearbyEnemies()
    {
        
        foreach (EnemyBrain enemyBrain in enemyReferences.enemyNavigation.GetNearbyEnemies())
        {            
            enemyBrain.engaged = true;
        }
        
    }
    private bool enteringBerserkState =false; //time span between non berserk and fully berserk state
    public void Berserk() => StartCoroutine(BerserkOn());
    //this should happen on the berserk state not here:
    private IEnumerator BerserkOn()
    {        
        isBerserk = true;
        //prevent other states from triggering when this is on
        enteringBerserkState = true;
        //invunerable during transition to berkserk
        enemyReferences.enemyHealth.invunerable = true;
        enemyReferences.enemyNavigation.StopNow(true);
        yield return null;
        enemyReferences.rb.useGravity = false;
        enemyReferences.rb.isKinematic = true;
        DisableColliders();
        yield return new WaitForFixedUpdate();
        enemyReferences.enemyAnimator.WarCry();
        shockwave.Play();
        burst.Play();
        enemyReferences.flash.Berserk();
        enemyReferences.enemySoundManager.PlayRoarSoundEffect();
        
        //enemyReferences.berserkParticles.Play();
        yield return new WaitForSeconds(2f);
        enemyReferences.enemyAnimator.Berserk();
        enemyReferences.enemyNavigation.Berserk();
        enemyReferences.enemyNavigation.StopNow(false);
        EnableColliders();
        enemyReferences.rb.useGravity = true;
        enemyReferences.rb.isKinematic = false;
        yield return new WaitForFixedUpdate();
        
        enteringBerserkState = false;
        enemyReferences.enemyHealth.invunerable = false;
        
        yield return null;       

    }
    public void Die()
    {
        dead = true;
        StopAllCoroutines();
        enemyReferences.enemyAnimator.Die();
        enemyReferences.rb.isKinematic = true;
        enemyReferences.rb.useGravity =false;
        DisableColliders();        
        StopRiggidbodyMovement();
        burst.Stop();
    }    
    float defaultStaggerTime;
    public void GotHit()
    {
        
        staggerTimmer = defaultStaggerTime;
        if (enemyReferences.enemyAnimator != null)
        {
            enemyReferences.enemyAnimator.Hit();
        }
        //allow for multihit        
        if (!wasHit) StartCoroutine(GotHitRot());
        //all hitting does is reset the timer;        
        
    }

    private IEnumerator GotHitRot()
    {
        wasHit = true;
        engaged = true;
        enemyReferences.rb.angularVelocity = Vector3.zero;
        while(staggerTimmer > 0f)
        {
            staggerTimmer -= Time.deltaTime;            
            yield return null;
        }
        wasHit = false;
    }

    //player detection
    private void ProbeSurroundings()
    {
        playerInsideTrigger = enemyReferences.enemyNavigation.PlayerInsideTriggerDistance();
       
        if (playerInsideTrigger || engaged)
        {
            //Only probe line of sight if:
            //  Player is inside sphere trigger
            //  This Enemy is activly engaged with the player (Meaning it spotted them and is either chasing or attacking the player)
            //reminder that the check is only done here:
            playerWithinLineOfSight = enemyReferences.enemyNavigation.HasLineOfSight(enemyReferences.playerRef.position, "Player");
            if (playerWithinLineOfSight)
            {
                //Enemy spots the player
                engaged = true;
                forgetTimmerCountdown = forgetTimmer;
                
            }
        }
        
        //if within attack range or if was hit 
        if (withinAttackRange || (!engaged && wasHit))
        {
            engaged = true;
            forgetTimmerCountdown = forgetTimmer;
            
        }
        //forgetting player after loosing sight:
        if (engaged && !playerWithinLineOfSight)
        {
            //Here have the enemy head to the last spotted location
            forgetTimmerCountdown -= Time.deltaTime;
            if (forgetTimmerCountdown <= 0f)
            {
                playerFirstSpoted = true;
                engaged = false;
                //might need to add a delay for this
                
            }

        }

        withinAttackRange = enemyReferences.enemyNavigation.LinearDistanceFromTarget(enemyReferences.playerRef.position) <= attackRange;

    }


    private void DisableColliders()
    {
        foreach (Collider col in colliders) col.enabled = false;
    }
    private void EnableColliders()
    {
        foreach (Collider col in colliders) col.enabled = true;
    }
    private void StopRiggidbodyMovement()
    {
        enemyReferences.rb.angularVelocity = Vector3.zero;
        enemyReferences.rb.linearVelocity = Vector3.zero;
    }
    
    
    private void OnDrawGizmos()
    {
        if (stateMachine != null)
        {
            Gizmos.color = stateMachine.GetGizmoColor();
            Gizmos.DrawSphere(transform.position + Vector3.up * 3, 0.4f);
        }
        if (!Application.isPlaying) return;
        
    }

    





}
