using System;
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
    //References
    private EnemyReferences enemyReferences;

    //Difference between IDDLE TO CHASE VS COMBAT TO CHASE is if the enemy was previously engaged or not. Or rather if they were "RECENTLY" ATTACKED. we need a new bool
    //Still needs:
    //PATHING CLASS
    //ANIMATOR CLASS
    //HEALTH CLASS

    //STATEMACHINE CLASS:
    private StateMachine stateMachine;
    
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
        At(idle, chase, () => PlayerDetected()); //This will update inside an enumerator. Ideally checked inside a fixedUpdate
        At(chase, idle, () => !playerInsideTrigger && !engaged);
        At(chase, combat, () => CloseEnoughToAttack()); //is within attackDistance
        At(combat, chase, () => !CloseEnoughToAttack() && engaged); //Outside attack range but still within line of sight
        //START STATE
        stateMachine.SetState(idle);

        //FUNCTIONS & CONDITIONS
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to,Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
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
        return enemyReferences.enemyNavigation.LinearDistanceFromTarget(player.position) <= attackRange;
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
    


   



}
