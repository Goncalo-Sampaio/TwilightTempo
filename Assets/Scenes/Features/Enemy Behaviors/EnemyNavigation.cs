using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour
{
    private NavMeshAgent agent;

    [Tooltip("Minimum distance from destination that the agent is considered as \"having arrived\"")]
    [SerializeField] private float arrivedDistance = 1f;
    private bool HasArrived()
    {
        return agent.remainingDistance <= arrivedDistance;
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        state = EState.Waiting;
    }
    private void Update()
    {
        //Wandering:
        
        if (state == EState.Waiting)
        {
            waitTime -= Time.deltaTime;
            if (waitTime < 0f)
            {
                ChangeState(EState.Wandering);
            }
        }
        else if (state == EState.Wandering)
        {
            wanderTime -= Time.deltaTime;
            if (HasArrived()|| wanderTime < 0f)
            {
                ChangeState (EState.Waiting);
            }

        }
    }
    //move this to the Idle/Patrol/Wander state class

    //Wandering:    
    [SerializeField] private Area area;
    [Tooltip("Timer before Wandering times out and a new destination is set. Used to prevent the agent getting stuck trying to reach a blocked destination")]
    [SerializeField] private float maxWanderTimer = 10f;
    private float wanderTime;
    public bool randomWaitTimes = false;

    [DisableIf("randomWaitTimes")][SerializeField] private float maxWaitTime = 2f;
    [EnableIf("randomWaitTimes")][MinMaxSlider(0.0f, 10.0f)][SerializeField] private Vector2 waitTimerRange;

    
    private float waitTime = 0f;
    private EState state;
    void SetRandomDestination()
    {
        agent.SetDestination(area.GetRandomPoint());
    }
    void ChangeState(EState targetState)
    {
        state = targetState; 
        if (state  == EState.Wandering)
        {
            agent.isStopped = false;
            SetRandomDestination();
            wanderTime = maxWanderTimer;

        }
        else if (state == EState.Waiting)
        {
            agent.isStopped = true;
            //Setting destination to the current agent position will stop the agent without the "autobreak slowdown":
            //agent.SetDestination(agent.transform.position);
            waitTime = randomWaitTimes ? Random.Range(waitTimerRange.x, waitTimerRange.y) : maxWaitTime;
        }
    }
    //We will be using states and not this
    enum EState
    {
        Wandering,
        Waiting
    }

}
