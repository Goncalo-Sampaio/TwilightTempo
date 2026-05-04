using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
/// <summary>
/// Navigation, Pathing and Query Interface
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavigation : MonoBehaviour
{
    [SerializeField] private bool debugger = true;
    [SerializeField] private float maxRayDistance = 100f;
    private NavMeshAgent agent;    
    private bool playerInsideTrigger = false;
    //[HideInInspector]public bool hasLineOfSight = false;
    [SerializeField] private Transform rayCastOrigin;
    [Tooltip("Minimum distance from destination that the agent is considered as \"having arrived\"")]
    [SerializeField] private float arrivedDistance = 1f;
    private float berserkSpeedIncrease = .5f;

    EnemyReferences enemyReferences;
    private Rigidbody rb;
    //Moving with phisics:
    private Vector3 currentTarget;
    [SerializeField] private bool desiredVelocity;
    [SerializeField] private bool steeringTarget;

    public bool moving;
    private bool HasArrived()
    {
        return agent.remainingDistance <= arrivedDistance;
    }
    private void Awake()
    {
        moving = false;
        enemyReferences = GetComponent<EnemyReferences>();
        rb = enemyReferences.rb;
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        //state = EState.Waiting;
        TogglePhysicsModeOn();

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
        if (!moving) return;
        MoveWithPhysics();
    }
    //Phisics:
    public void TogglePhysicsModeOn()
    {
        Debug.Log("TogglePhysicsModeOn");
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        rb.isKinematic = false;
        rb.useGravity = true;
    }
    private void MoveWithPhysics()
    {
        
        // force the navmeshagent velocity to match the rigidbody
        agent.velocity = rb.linearVelocity;

        // navMeshAgent.desiredVelocity is the velocity the navmesh agent needs to do its AI behaviour
        //Its a result of it's current velocity towards the next position + the avoidance contribution.
        //The "go here now" vector at the current frame
        var desiredVelocity = agent.desiredVelocity; //Debug this

        //calculate a force to accelerate the rigidbody so that its velocity moves toward the velocity the navmesh agent wants to have to follow its path
        var calculatedForce = CalculateForceNeededToReachDesiredVelocity(desiredVelocity);
        rb.AddForce(calculatedForce, ForceMode.Force);

        //Synch Riggidbody with agent:
        agent.nextPosition = rb.position;

        //Look at target so that it stops jittering the rotation
        //However switch this out with a smooth rotation towards the target rather.
        Vector3 direction = (currentTarget - transform.position);
        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);

        Debug.Log(GetVisionConeFactor(currentTarget));
        if (GetVisionConeFactor(currentTarget) < .65)
        {
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(flatDirection.normalized, transform.up), 0.5f);
        }
        else rb.rotation = Quaternion.LookRotation(flatDirection.normalized, transform.up);

    }
    private Vector3 CalculateForceNeededToReachDesiredVelocity(Vector3 desiredVelocity)
    {
        // Calculate force needed to reach targetVelocity in the next fixed update
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 acceleration = (desiredVelocity - currentVelocity) / Time.fixedDeltaTime;
        return rb.mass * acceleration;
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

    public void Berserk()
    {
        agent.speed += agent.speed * berserkSpeedIncrease;
    }
    public bool IsAgentOnNavmesh() => agent.isOnNavMesh;
    public bool IsAgentStopped() => agent.isStopped;
    public bool IsAgentActive() => agent.enabled;
    public void ToggleAgentStopped(bool toggle)
    {
        if(toggle) agent.velocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        agent.isStopped = toggle;
        //agent.path = null;

    }
    public void ToggleEnableAgent(bool toggle) => agent.enabled = toggle;
    //Updating this:
    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
        currentTarget = destination;
    }

    public void Warp(Vector3 position) => agent.Warp(position);
    //Only use in states that are not "Chase". MoveWithPhysics already orients orc agent towards player.
    public void LookAtTarget(Vector3 target)
    {
        /////return;
        //var q = Quaternion.LookRotation(target - transform.position);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 100f * Time.deltaTime);
        Vector3 direction = (target - transform.position);
        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);

        Debug.Log(GetVisionConeFactor(target));
        if (GetVisionConeFactor(target) < .65)
        {
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(flatDirection.normalized, transform.up), 0.5f);
        }
        else rb.rotation = Quaternion.LookRotation(flatDirection.normalized, transform.up);
    }
    
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
    private Vector3 targetDirectionDebugg;
    public float GetVisionConeFactor(Vector3 targetPos)
    {
        //Remember kids always debug your lines:
        //I forget but this is a 3D vector and im used to checking dotproduct on a flat plane
        //Also the rayorigin is in front of the orc's head. That means that if the player gets behind it by going bellow the orc the dot will return - 1
        //New rule. For raycasts where i need to actually know if the player is in front of the orc's eyes then yeah use the raycastorigin object as a source
        //Otherwise if i just want pure position then use transform.positon.
        Vector3 targetDirection = (targetPos - transform.position);
        Vector3 flatTargetDirection = new Vector3(targetDirection.x,0, targetDirection.z).normalized;
        targetDirectionDebugg = currentTarget;
        return Vector3.Dot(transform.forward, flatTargetDirection);
    }
    public void StopNow(bool stop)
    {
        if (IsAgentActive())
        {
            Debug.Log("agent.active == true");
            if (IsAgentOnNavmesh())
            {
                Debug.Log("agent.IsOnNavmesh == true");
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
        Debug.Log(other.name);
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Player")
        {
            playerInsideTrigger = true;            
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") playerInsideTrigger = false;
    }
    #endregion

    #region Debugging
    private bool debug = true;
    private void OnDrawGizmos()
    {
        if (!debug) return;
        {
            Gizmos.color = Color.purple;
            Gizmos.DrawLine(transform.position, transform.position + targetDirectionDebugg);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
        }
    }
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
