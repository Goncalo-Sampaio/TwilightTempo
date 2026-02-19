using System.Collections; 
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class NavigationTesting : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 currentTarget;
    private bool isChassing, gotHit;
    private Rigidbody rb;

    [SerializeField]private float chaseUpdateFrequency;
    private float chaseTimer;
    private float gotHitTimer;

    [SerializeField] private LayerMask playerDamageLayer;
    [SerializeField] private Transform player;
    [SerializeField] private float detectionDistance = 5f;
    [Header("Debugg Gizmos")]
    [SerializeField] private bool desiredVelocity;
    [SerializeField] private bool steeringTarget;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        chaseTimer = chaseUpdateFrequency;
        gotHitTimer = 3f;

    }
    private void Start()
    {
        TogglePhysicsModeOn();
    }

    public void SetPlayer(Transform target) => player = target ;
    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
        currentTarget = destination;
    }
    public void TogglePhysicsModeOn()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        rb.isKinematic = false;
        rb.useGravity = true;
    }
    private void FixedUpdate()
    {
        //If hit by player freeze all movement and take the hit:
        if (gotHit)
        {
            // take it like a man
            //wait 3 seconds and chase em again
            gotHit = !GotHitUpdate();
        }
        else
        {
            //Player Detection:
            //if equal or within detection distance isChassing = True
            isChassing = Vector3.Distance(transform.position, player.position) <= detectionDistance;

            if (isChassing)
            {
                //Update path on a timer
                if (ChaseUpdate()) MoveTo(player.position);

            }
            //Move agent using RiggidBody
            MoveWithPhysics();
        }
        

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
        rb.rotation = Quaternion.LookRotation(flatDirection.normalized, transform.up);
    }
    private Vector3 CalculateForceNeededToReachDesiredVelocity(Vector3 desiredVelocity)
    {
        // Calculate force needed to reach targetVelocity in the next fixed update
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 acceleration = (desiredVelocity - currentVelocity) / Time.fixedDeltaTime;
        return rb.mass * acceleration;
    }
    // we can prob make a timer method:
    private bool ChaseUpdate()
    {
        chaseTimer -= Time.deltaTime;
        if (chaseTimer <= 0f)
        {
            chaseTimer = chaseUpdateFrequency;
            return true;
        }
        else return false;
    }
    private bool GotHitUpdate()
    {
        gotHitTimer -= Time.deltaTime;
        if (gotHitTimer <= 0f)
        {
            gotHitTimer = 3f;
            return true;
        }
        else return false;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        //if other collider is on player layer
        if ((playerDamageLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            gotHit = true;
            TakeDamage(10f,( transform.position - player.position + transform.up * .4f).normalized * 10f);
        }
    }
    //This should be called on player's weapon script not here
    public void TakeDamage(float damage,Vector3 knockbackForce)
    {
        rb.AddForce(knockbackForce, ForceMode.Impulse);
    }
    private void OnDrawGizmos()
    {
        if (desiredVelocity)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (agent.desiredVelocity));

        }
        if(steeringTarget)
        {
            Gizmos.color = Color.purple;
            Gizmos.DrawLine(transform.position, (agent.steeringTarget));
        }
        
    }

}
