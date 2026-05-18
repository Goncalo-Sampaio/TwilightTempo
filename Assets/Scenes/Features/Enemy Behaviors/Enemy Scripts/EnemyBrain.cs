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
    private bool playerWasSpoted;
    [HideInInspector] public bool engaged = false;
    [SerializeField] private float forgetTimmer = 5f;
    private float forgetTimmerCountdown;
    private Collider[] colliders;


    private void Awake()
    {
        playerWasSpoted = false;
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
        At(chase, idle, () => !engaged && !berserkLag && !dead);        
        At(combat, chase, () => engaged && !withinAttackRange && !dead);         
        Any(gotHit, () => wasHit && !dead);
        Any(death, () => dead);
        Any(combat, () => withinAttackRange && engaged && !dead);

        At(gotHit, chase, ()=> !wasHit && engaged && !dead);
        At(gotHit, combat, () => !wasHit && withinAttackRange && engaged && !dead);
        Any(berserk, () => berserkLag && !dead);
        At(berserk, chase, () => !berserkLag && engaged && !dead);
        At(berserk, combat, () => !berserkLag && !wasHit && withinAttackRange && engaged && !dead);
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
            if(forgetTimmerCountdown <= 0f) engaged = false;
        } 
        //if within attack range or if was hit 
        if(withinAttackRange || (!engaged && wasHit))
        {
            engaged = true;
            forgetTimmerCountdown = forgetTimmer;
        }
        
        withinAttackRange = enemyReferences.enemyNavigation.LinearDistanceFromTarget(enemyReferences.playerRef.position) <= attackRange;

    }
    private bool berserkLag =false;
    public void Berserk() => StartCoroutine(BerserkOn());
    private IEnumerator BerserkOn()
    {
        isBerserk = true;
        berserkLag = true;
        enemyReferences.enemyNavigation.StopNow(true);
        yield return null;
        enemyReferences.rb.useGravity = false;
        enemyReferences.rb.isKinematic = true;
        DisableColliders();
        yield return new WaitForFixedUpdate();
        enemyReferences.enemyAnimator.WarCry();
        
        enemyReferences.berserkParticles.Play();
        yield return new WaitForSeconds(2f);
        enemyReferences.enemyAnimator.Berserk(1.2f);
        enemyReferences.enemyNavigation.Berserk();
        enemyReferences.enemyNavigation.StopNow(false);
        EnableColliders();
        enemyReferences.rb.useGravity = true;
        enemyReferences.rb.isKinematic = false;
        yield return new WaitForFixedUpdate();
        berserkLag = false;
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
