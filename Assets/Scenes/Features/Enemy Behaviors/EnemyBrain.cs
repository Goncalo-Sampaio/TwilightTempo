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
    public float forgetTimmer = 5f;
    private float forgetTimmerCountdown;
    [Header("CHASE params")]
    [SerializeField] private Transform player;
    [SerializeField] private float chaseUpdateFrequency = 0.2f;

    private bool playerInsideTrigger = false;

    [Header("COMBAT params")]
    [SerializeField] private float attackUpdateFrequency = 0.2f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRangeTolerance = .3f;
    [SerializeField] private float maxKnockBackTime = .5f;
    //References
    private EnemyReferences enemyReferences;

    //Difference between IDLE TO CHASE VS COMBAT TO CHASE is if the enemy was previously engaged or not. Or rather if they were "RECENTLY" ATTACKED. we need a new bool
    //Still needs:
    //PATHING CLASS
    //ANIMATOR CLASS
    //HEALTH CLASS

    //STATEMACHINE CLASS:
    private StateMachine stateMachine;

    //Local vars
    private bool gettingKnockBacked = false;
    private float triggerColliderRadius;
    private float groundOffset;
    
    private void OnValidate()
    {
        groundOffset = GetComponent<CapsuleCollider>().height / 2;
        triggerColliderRadius = GetComponent<SphereCollider>().radius;
    }
    private void Start()
    {
        forgetTimmerCountdown = forgetTimmer;
        enemyReferences = GetComponent<EnemyReferences>();
        stateMachine = new StateMachine();

        //STATES
        var idle = new EnemyState_Idle(enemyReferences);
        var chase = new EnemyState_Chase(enemyReferences, player, chaseUpdateFrequency);
        var combat = new EnemyState_Combat(enemyReferences, player, attackUpdateFrequency); 
        var delay = new EnemyState_Delay(2f);

        //TRANSITIONS
        At(idle, chase, () => PlayerDetected() && !gettingKnockBacked); //This will update inside an enumerator. Ideally checked inside a fixedUpdate
        At(chase, idle, () => !playerInsideTrigger && !engaged && !gettingKnockBacked);
        At(chase, combat, () => CloseEnoughToAttack() && !gettingKnockBacked); //is within attackDistance
        At(combat, chase, () => !CloseEnoughToAttack() && engaged && !gettingKnockBacked); //Outside attack range but still within line of sight
        //(delay,() => enemyReferences.enemyHealth.dead);
        //Make a knockedback STATE
        Any(delay, () => gettingKnockBacked);
        //START STATE
        stateMachine.SetState(idle);

        //FUNCTIONS & CONDITIONS
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to,Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
    }
    [Button]
    public void KnockTest() => KnockTest(200f * -transform.forward);
    public void KnockTest(Vector3 force) => StartCoroutine("ApplyKnockBack", force);
    private IEnumerator ApplyKnockBack(Vector3 force)
    {
        gettingKnockBacked = true;

        yield return null; //wait one frame to make sure all courotines are stopped
        enemyReferences.enemyNavigation.ToggleEnableAgent(false);
        enemyReferences.GetComponent<Rigidbody>().useGravity = true;
        enemyReferences.GetComponent<Rigidbody>().isKinematic = false;
        enemyReferences.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        //only exit after the fixedUpdate frame is passed. To make sure the force is applied
        yield return new WaitForFixedUpdate();
        float knockBackTime = Time.time; 
        yield return new WaitUntil(() => enemyReferences.GetComponent<Rigidbody>().linearVelocity.magnitude < 0.05f || Time.time > knockBackTime + maxKnockBackTime); //wait until it stops moving.

        yield return new WaitForSeconds(0.25f); //stun frames //consider adding a flash here

        //now reset:
        enemyReferences.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        enemyReferences.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        enemyReferences.GetComponent<Rigidbody>().useGravity = false;
        enemyReferences.GetComponent<Rigidbody>().isKinematic = true;

        //snap agent back to navmesh
        enemyReferences.enemyNavigation.Warp(transform.position);
        //THEN AND ONLY THEN
        //enable the agent
        enemyReferences.enemyNavigation.ToggleEnableAgent(true);

        gettingKnockBacked = false;
    }

    /// <summary>
    /// Player is inside enemie's trigger distance AND has line of sight
    /// </summary>
    /// <returns></returns>
    private bool PlayerDetected()
    {        
        if (playerInsideTrigger) return enemyReferences.enemyNavigation.HasLineOfSight(player.position);
        else return false;
    }
    private bool CloseEnoughToAttack()
    {
        float distanceToPlayer = enemyReferences.enemyNavigation.LinearDistanceFromTarget(player.position);

        bool isInsideRange = distanceToPlayer < attackRange + attackRangeTolerance && distanceToPlayer > attackRange - attackRangeTolerance;
        Debug.Log($"Min = {attackRange - attackRangeTolerance} Current = {distanceToPlayer}, Max = {attackRange + attackRangeTolerance}, Is inside range = {isInsideRange}, engaged = {engaged} ");

        return isInsideRange;
    }



    private void Update()
    {
        stateMachine.Tick();
        ShortTermMemory();


    }
    private void FixedUpdate()
    {
        //OnPlayerDetected = OnPlayerVisible();
        playerInsideTrigger = enemyReferences.enemyNavigation.PlayerInsideChaseDistance();


    }
    private void ShortTermMemory()
    {
        
        //Reset timer
        if (PlayerDetected())
        {
            engaged = true;
            forgetTimmerCountdown = forgetTimmer;
        }
        else
        {
            forgetTimmerCountdown -= Time.deltaTime;
        }
        if (forgetTimmerCountdown <= 0f) engaged = false;
        Debug.Log($"Forget me timer: {forgetTimmerCountdown}");

    }

    private void OnDrawGizmos()
    {
        if (stateMachine != null)
        {
            Gizmos.color = stateMachine.GetGizmoColor();
            Gizmos.DrawSphere(transform.position + Vector3.up * 3, 0.4f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Handles.Label(transform.position + Vector3.up * (chaseTriggerDistance + .2f), $"Trigger Distance = {chaseTriggerDistance}");
        
        Gizmos.DrawWireSphere(transform.position + Vector3.up * -groundOffset, triggerColliderRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * -groundOffset, attackRange - attackRangeTolerance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * -groundOffset, attackRange + attackRangeTolerance);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(attackPoint.position, attackPoint.position + debbugAttackDir * 2);
    }





}
