using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Navigation, Pathing and Query Interface
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavigation : MonoBehaviour
{
    [SerializeField] private bool debugger = true;
    private NavMeshAgent agent;    
    private bool playerInsideTrigger = false;
    //[HideInInspector]public bool hasLineOfSight = false;
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
        //state = EState.Waiting;
    }

    private void Update()
    {
        ////Wandering:
        
        //if (state == EState.Waiting)
        //{
        //    waitTime -= Time.deltaTime;
        //    if (waitTime < 0f)
        //    {
        //        ChangeState(EState.Wandering);
        //    }
        //}
        //else if (state == EState.Wandering)
        //{
        //    wanderTime -= Time.deltaTime;
        //    if (HasArrived()|| wanderTime < 0f)
        //    {
        //        ChangeState (EState.Waiting);
        //    }

        //}
    }
    private void FixedUpdate()
    {
        
    }
    //move this to the Idle/Patrol/Wander state class

    ////Wandering:    
    //[SerializeField] private Area area;
    //[Tooltip("Timer before Wandering times out and a new destination is set. Used to prevent the agent getting stuck trying to reach a blocked destination")]
    //[SerializeField] private float maxWanderTimer = 10f;
    //private float wanderTime;
    //public bool randomWaitTimes = false;

    //[DisableIf("randomWaitTimes")][SerializeField] private float maxWaitTime = 2f;
    //[EnableIf("randomWaitTimes")][MinMaxSlider(0.0f, 10.0f)][SerializeField] private Vector2 waitTimerRange;

    public void ToggleAgentStart(bool toggle) => agent.isStopped = toggle;   
    public void ToggleEnableAgent(bool toggle) => agent.enabled = toggle;
    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void Warp(Vector3 position) => agent.Warp(position);
    
    public void LookAtTarget(Vector3 target) => transform.LookAt(target);
    
    public float NavMeshDistanceFromTarget() => agent.remainingDistance;

    public float LinearDistanceFromTarget(Vector3 target) => Vector3.Distance(transform.position, target);

    //Only call this if "playerInsideTrigger" is true
    //This has to update a bool inside a FixedUpdate
    //
    public bool HasLineOfSight(Vector3 targetPos, string targetTag = "Player")
    {
        Debug.Log("HasLineOfSight Called");
            //Make sure to also include line of sight mwaybe? Using the dotP
            RaycastHit hit;
            
            //if hits anything
            if (Physics.Raycast(transform.position, (targetPos - transform.position).normalized, out hit, Mathf.Infinity) && (hit.transform.gameObject.tag == targetTag))
            {
                //if hits object tagged with "Player"
                if (hit.transform.gameObject.tag == targetTag)
                {
                    if (debugger) DebugLineOfSight(true, targetPos, hit);
                    return true;
                }
            }
            //if hit nothing:
            if (debugger) DebugLineOfSight(false, targetPos, hit);

            
            return false;


        
    }
    public bool PlayerInsideChaseDistance() => playerInsideTrigger;
    
    public bool HasArrivedAtTarget(float minDistance = 0.1f)
    {
        return NavMeshDistanceFromTarget() < minDistance;
    }

    #region Trigger Events

    //Detection Sphere Trigger:
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") playerInsideTrigger = true;

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") playerInsideTrigger = false;
    }
    #endregion

    #region Debugging
    private void DebugLineOfSight(bool triggered, Vector3 target, RaycastHit hit)
    {
        if (triggered)
        {
            Debug.DrawRay(transform.position, (target - transform.position).normalized * hit.distance, Color.yellow);
            Debug.Log("Player Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("No line of sight");
        }

    }
    #endregion

    //private float waitTime = 0f;
    //private EState state;
    //void SetRandomDestination()
    //{
    //    agent.SetDestination(area.GetRandomPoint());
    //}


    //void ChangeState(EState targetState)
    //{
    //    state = targetState; 
    //    if (state  == EState.Wandering)
    //    {
    //        agent.isStopped = false;
    //        SetRandomDestination();
    //        wanderTime = maxWanderTimer;

    //    }
    //    else if (state == EState.Waiting)
    //    {
    //        agent.isStopped = true;
    //        //Setting destination to the current agent position will stop the agent without the "autobreak slowdown":
    //        //agent.SetDestination(agent.transform.position);
    //        waitTime = randomWaitTimes ? Random.Range(waitTimerRange.x, waitTimerRange.y) : maxWaitTime;
    //    }
    //}
    ////We will be using states and not this
    //enum EState
    //{
    //    Wandering,
    //    Waiting
    //}

}
