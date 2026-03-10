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
    [SerializeField] private Transform rayCastOrigin;
    [SerializeField] private float maxRayDistance = 100f;
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

    public bool IsAgentOnNavmesh() => agent.isOnNavMesh;
    public bool IsAgentStopped() => agent.isStopped;
    public bool IsAgentActive() => agent.enabled;
    public void ToggleAgentStopped(bool toggle)
    {
        if(true) agent.velocity = Vector3.zero;
        agent.isStopped = toggle;

    }
    public void ToggleEnableAgent(bool toggle) => agent.enabled = toggle;
    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void Warp(Vector3 position) => agent.Warp(position);

    public void LookAtTarget(Vector3 target)
    {
        var q = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 35f * Time.deltaTime);
    }
    public void SnapToTarget(Vector3 target) => transform.LookAt(target);

    public float NavMeshDistanceToDestination() => agent.remainingDistance;

    //this should only be valid if there's no obstruction == line of sight = true;
    public float LinearDistanceFromTarget(Vector3 target) => Vector3.Distance(new Vector3(transform.position.x,0, transform.position.z), new Vector3(target.x,0,target.z));

    //Only call this if "playerInsideTrigger" is true    
    //Can look for other things besides player
    public bool HasLineOfSight(Vector3 targetPos, string targetTag = "Player")
    {
        Vector3 targetDirection = (targetPos - rayCastOrigin.position).normalized;

        //Only try casting if target is infront
        if (Vector3.Dot(rayCastOrigin.forward, targetDirection) < 0f) return false;
        
        //Make sure to also include line of sight mwaybe? Using the dotP
        RaycastHit hit;
        //if hits anything
        if (Physics.Raycast(rayCastOrigin.position, targetDirection, out hit, maxRayDistance))
        {
            //if hits object tagged with "targetTag"
            if (hit.transform.gameObject.tag == targetTag) return true;
        }

        //if hit nothing:            
        return false;

    }
    public void StopNow(bool stop)
    {
        if (IsAgentActive())
        {
            if (IsAgentOnNavmesh())
            {
                //Prevent error:
                //The agent.isStopped getter can only be called if the agent.active == true && agent.IsOnNavmesh == true:
                ToggleAgentStopped(stop);//stop agent navmesh
                Debug.Log("ToggleAgentStopped called!");
            }
        }
    }
    public bool PlayerInsideTriggerDistance() => playerInsideTrigger;
    
    public bool HasArrivedAtTarget(float minDistance = 0.1f)
    {
        return NavMeshDistanceToDestination() < minDistance;
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
