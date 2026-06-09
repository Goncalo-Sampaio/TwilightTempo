using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public NavMeshAgent agent;    
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
    private Vector3 desiredVelocity;


    [SerializeField] private bool isOnIce = false;
    [SerializeField] private float iceForceMultiplier = 0.2f;
    [SerializeField] private float iceLinearDamp = 0.5f;
    [SerializeField] private float defaultLinearDamp = 10;

    [SerializeField] private float maxAcceleration = 50f;
    [SerializeField] private float iceAcceleration = 10f;

    [SerializeField] private float avoidFactor = 5f;
    [SerializeField] private float avoidPDistance = .5f; //how far away to the nearest agent to start avoiding
    private List<EnemyBrain> nearbyEnemies = new();
    public bool moving;
    private bool HasArrived()
    {
        return agent.remainingDistance <= arrivedDistance;
    }
    private void Awake()
    {
        moving = false;
        enemyReferences = GetComponent<EnemyReferences>();
        
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        NavRayCastPosition = transform.position;
        //state = EState.Waiting;
        rb = enemyReferences.rb;
        TogglePhysicsModeOn();
        NavMesh.avoidancePredictionTime = 0.5f ;

    }   
    private void FixedUpdate()
    {
        
        if (!moving)
        {
            //stopped and attacking agents can't be pushed since they have higher priority:
            if(enemyReferences.enemyBrain.engaged) agent.avoidancePriority = 45;
            return;
        }
        
        agent.avoidancePriority = 50;
        MoveWithPhysics();
        ResynchAgent();
        //Debugging:
        if (agent.pathStatus == NavMeshPathStatus.PathPartial) Debug.Log("NavMeshPathStatus.PathPartial");
        if (agent.pathPending) Debug.Log("agent.pathPending");
        if (!agent.hasPath) Debug.Log("Agent has no path");

    }
    //Phisics:
    public void TogglePhysicsModeOn()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        rb.isKinematic = false;
        rb.useGravity = true;
    }
    

    private void MoveWithPhysics()
    {
        //rb.linearVelocity is what the phisics is pushing on the agent. This includes being shoved, slidding collisions etc.
        //agent.desiredVelocity is what the agent "intends" to do. Discounting phisics just the pathfinding's result with avoidance and all that


        //update agent's simulation with ridibody's simulation value
        agent.velocity = rb.linearVelocity;
        
        //get the target velocity towards our destination(this includes any avoidance)
        desiredVelocity = agent.desiredVelocity;

        //if on ice then constantly blend (play catchup) from the phisics simulation and the "intended velocity"
        //this causes the agent to constantly try to fight against the ice sliding catching up to its intended velocity.
        //This only blends the velocity vector
        if (isOnIce)
        {
            //with lerp  of 0.1 - Think of it like trying to controll a very heavy car with agreesive inertia in a phisics game 
            desiredVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, 0.1f);
        }
        //always accelerates towards the intended velocity at "maxAcceleration"
        //The output is the same as the ice phisics example from above but this actually uses a input acceleration value.
        //This is meant to stack with the ice contribution
        //prevents spikes
        Vector3 targetVelocity = Vector3.MoveTowards(rb.linearVelocity, desiredVelocity, maxAcceleration * Time.fixedDeltaTime);

        //After getting the target velocity we want to actully apply it to the riggibody.
        //Since we want to use AddForce its better to first derive the acceleration we want and use that instead:
        var desiredAcceleration = (targetVelocity - rb.linearVelocity) / Time.fixedDeltaTime;

        float accelLimit = isOnIce ? iceAcceleration : maxAcceleration;
        desiredAcceleration = Vector3.ClampMagnitude(desiredAcceleration, accelLimit);

        //smoth it out
        rb.AddForce(desiredAcceleration * rb.mass, ForceMode.Force);
        rb.linearDamping = isOnIce ? iceLinearDamp : defaultLinearDamp;
        
        //updates the agent's position in the navmesh simulation to the current position of the riggidbody
        agent.nextPosition = rb.position;

        //Rotates towards target:
        Vector3 direction = (currentTarget - transform.position);
        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);

        if (flatDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection.normalized, transform.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 0.15f);
        }
    }
    private void MoveWithPhysicsbackup()
    {
        agent.velocity = rb.linearVelocity;

        desiredVelocity = agent.desiredVelocity;

        if (isOnIce)
        {
            desiredVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, 0.1f);
        }
        //prevents spikes
        Vector3 targetVelocity = Vector3.MoveTowards(rb.linearVelocity, desiredVelocity, maxAcceleration * Time.fixedDeltaTime);


        var desiredAcceleration = (targetVelocity - rb.linearVelocity) / Time.fixedDeltaTime;

        float accelLimit = isOnIce ? iceAcceleration : maxAcceleration;
        desiredAcceleration = Vector3.ClampMagnitude(desiredAcceleration, accelLimit);

        //smoth it out
        rb.AddForce(desiredAcceleration * rb.mass, ForceMode.Force);
        rb.linearDamping = isOnIce ? iceLinearDamp : defaultLinearDamp;


        agent.nextPosition = rb.position;


        Vector3 direction = (currentTarget - transform.position);
        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);

        if (flatDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection.normalized, transform.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 0.15f);
        }
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
        if (!agent.isOnNavMesh) Warp(transform.position);
        agent.SetDestination(destination);
        currentTarget = destination;
    }
    
    public void Warp(Vector3 position) => agent.Warp(position);
    //Only use in states that are not "Chase". MoveWithPhysics already orients orc agent towards player.
    public void LookAtTarget(Vector3 target)
    {       
        Vector3 direction = (target - transform.position);
        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);

        
        if (GetVisionConeFactor(target) < .65)
        {
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(flatDirection.normalized, transform.up), 0.5f);
        }
        else rb.rotation = Quaternion.LookRotation(flatDirection.normalized, transform.up);
    }
    
    public float NavMeshDistanceToDestination() => agent.remainingDistance;

    public bool hasLineOfSight = false;
    //this should only be valid if there's no obstruction == line of sight = true;
    public float LinearDistanceFromTarget(Vector3 target) => Vector3.Distance(new Vector3(transform.position.x,0, transform.position.z), new Vector3(target.x,0,target.z));

    //Only call this if "playerInsideTrigger" is true    
    //Can look for other things besides player
    bool somethingHit;
    RaycastHit hitData;
    public LayerMask ignoreThisLayer;
    public bool HasLineOfSight(Vector3 targetPos, string targetTag = "Player")
    {        
        Vector3 targetDirection = (targetPos - rayCastOrigin.position).normalized;

        //Only try casting if target is infront
        if (Vector3.Dot(rayCastOrigin.forward, targetDirection) < 0f) 
        {
            hasLineOfSight = false;
            return false; }

        //Make sure to also include line of sight mwaybe? Using the dotP
        
        RaycastHit hit;
        //if hits anything
        if (Physics.Raycast(rayCastOrigin.position, targetDirection, out hit, maxRayDistance, ~ignoreThisLayer))
        {
            somethingHit = true;
            hitData = hit;
            //if hits object tagged with "targetTag"
            if (hit.transform.gameObject.tag == targetTag && hit.transform.GetComponentInParent<PlayerHealth>() != null)
            {
                hasLineOfSight = true;
                return true;
            }
        }
        else
        {
            hasLineOfSight = false;
            somethingHit = false;
        }
        hasLineOfSight = false;

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
            if (IsAgentOnNavmesh())
            {
                //Prevent error:
                //The agent.isStopped getter can only be called if the agent.active == true && agent.IsOnNavmesh == true:
                ToggleAgentStopped(stop);//stop agent navmesh
                
            }
        }
    }
    private Vector3 NavRayCastPosition;
    public bool PlayerInsideTriggerDistance() => playerInsideTrigger;
    private void ResynchAgent()
    {
        
        float desynchThreshold = .5f;
        Vector3 desynchVector = agent.nextPosition - rb.position;

        if (desynchVector.sqrMagnitude > (desynchThreshold * desynchThreshold))
        {
            // Use rb.position to find the nearest valid navmesh point
            if (NavMesh.SamplePosition(rb.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                NavRayCastPosition = hit.position;

                //MovePosition keeps physics velocities intact and stops jitter - use this instead to not break phisc
                rb.MovePosition(hit.position);
                agent.Warp(hit.position); // Ensure the agent's internal position resets too
            }
            else
            {
                rb.MovePosition(agent.nextPosition);
                rb.linearVelocity = Vector3.zero; // Kill velocity so it doesn't shoot away
            }
        }
    }
    public bool HasArrivedAtTarget()
    {
        return NavMeshDistanceToDestination() < agent.stoppingDistance;
    }

    #region Trigger Events

    //Detection Sphere Trigger:
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) playerInsideTrigger = true;
        if (other.gameObject.CompareTag("Enemy")) AddAndSort(other.GetComponentInParent<EnemyBrain>());

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) playerInsideTrigger = false;
        if (other.gameObject.CompareTag("Enemy")) nearbyEnemies.Remove(other.GetComponentInParent<EnemyBrain>());
    }
    //Add new enemy and sort List by distance
    private void AddAndSort(EnemyBrain enemy)
    {
        if (nearbyEnemies.Contains(enemy)) return;
        nearbyEnemies.Add(enemy);
        //Sort by distance: https://www.youtube.com/watch?v=7EALNQ9tFlw&t=224s
        nearbyEnemies.Sort((a, b) => Vector3.SqrMagnitude(b.transform.position - a.transform.position)
        .CompareTo(Vector3.SqrMagnitude(a.transform.position - transform.position)));
    }
    public List<EnemyBrain> GetNearbyEnemies() { return nearbyEnemies; }
    #endregion

    #region Debugging
    private bool debug = true;
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(agent.nextPosition, .2f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(NavRayCastPosition, .2f);

        Gizmos.color = Color.purple;
        Gizmos.DrawLine(transform.position, transform.position + targetDirectionDebugg);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + desiredVelocity);

        if (!debug) return;
        {
            
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawLine(transform.position, transform.position + transform.forward);
            //Gizmos.color = Color.red;
            //Gizmos.DrawLine(transform.position, transform.position + desiredVelocity);
            
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
