using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
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

    private bool playerWithinLineOfSight, withinAttackRange;
    private bool playerFirstSpoted;
    [HideInInspector] public bool engaged = false;    
    [SerializeField] private float forgetTimmer = 5f;
    private float forgetTimmerCountdown;
    private Collider[] colliders;

    [SerializeField] private VisualEffect burst;
    [SerializeField] private ParticleSystem shockwave;
    private void Awake()
    {
        playerFirstSpoted = true;
        stateMachine = new StateMachine();
        enemyReferences = GetComponent<EnemyReferences>();
        colliders = GetComponentsInChildren<Collider>();
    }

    private void Start()
    {

        groundOffset = GetComponentInChildren<CapsuleCollider>().height / 2;        
        
        //STATES
        var idle = new EnemyState_Idle(enemyReferences);
        var chase = new EnemyState_Chase(enemyReferences, chaseUpdateFrequency);
        var combat = new EnemyState_Combat(enemyReferences, attackUpdateFrequency);
        var gotHit = new EnemyState_GotHit(enemyReferences);
        var death = new EnemyState_Death();
        var berserk = new EnemyState_Berserk(enemyReferences);
        //TRANSITIONS
        At(idle, chase, () => engaged && !dead); 
        At(chase, idle, () => !engaged && !enteringBerserkState && !dead);        
        At(combat, chase, () => engaged && !withinAttackRange && !dead);  
        At(gotHit, chase, ()=> !wasHit && engaged && !dead);
        At(gotHit, combat, () => !wasHit && withinAttackRange && engaged && !dead);        
        At(berserk, chase, () => !enteringBerserkState && engaged && !dead);
        At(berserk, combat, () => !enteringBerserkState && !wasHit && withinAttackRange && engaged && !dead);

        Any(gotHit, () => wasHit && !dead && !enteringBerserkState);
        Any(death, () => dead);
        Any(combat, () => withinAttackRange && engaged && !dead && !enteringBerserkState);
        Any(berserk, () => enteringBerserkState && !dead);

        //START STATE
        stateMachine.SetState(idle);

        //FUNCTIONS & CONDITIONS
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to,Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
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
        //forgetting player after loosing sight:
        if(engaged && !playerWithinLineOfSight)
        {
            forgetTimmerCountdown -= Time.deltaTime;
            if (forgetTimmerCountdown <= 0f)
            {
                playerFirstSpoted = true;
                engaged = false;
            }
            
        } 
        //if within attack range or if was hit 
        if(withinAttackRange || (!engaged && wasHit))
        {
            engaged = true;
            forgetTimmerCountdown = forgetTimmer;
        }
        
        withinAttackRange = enemyReferences.enemyNavigation.LinearDistanceFromTarget(enemyReferences.playerRef.position) <= attackRange;

    }
    private bool enteringBerserkState =false; //time span between non berserk and fully berserk state
    public void Berserk() => StartCoroutine(BerserkOn());
    //this should happen on the berserk state not here:
    private IEnumerator BerserkOn()
    {
        Debug.Log("Berserk Routine started");
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
        Debug.Log("Berserk Routine ended");

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
    public void GotHit()
    {
        if (!wasHit) StartCoroutine(GotHitRot());
    }
    private IEnumerator GotHitRot()
    {
        wasHit = true;
        engaged = true;
        enemyReferences.rb.angularVelocity = Vector3.zero;
        yield return new WaitForSeconds(staggerTimmer);
        wasHit = false;
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
    private void Update()
    {
        stateMachine.Tick();
        
    }
    private void FixedUpdate()
    {
        ProbeSurroundings();
        if (engaged)
        {
            if (playerFirstSpoted)
            {
                enemyReferences.enemySoundManager.PlaySpottedPlayerSoundEffect();
                playerFirstSpoted = false;
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (stateMachine != null)
        {
            Gizmos.color = stateMachine.GetGizmoColor();
            Gizmos.DrawSphere(transform.position + Vector3.up * 3, 0.4f);
        }
    }

    





}
