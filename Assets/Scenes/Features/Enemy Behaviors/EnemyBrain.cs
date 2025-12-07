using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBrain : MonoBehaviour
{
    //Handles high level logic. and calls subcomponent methods    
    //Pools data from other subcomponents to execute desicions

    //References
    private EnemyReferences enemyReferences;
    //Still needs:
    //PATHING CLASS
    //ANIMATOR CLASS
    //HEALTH CLASS

    //STATEMACHINE CLASS:
    private StateMachine stateMachine;
    
    private void Start()
    {
        enemyReferences = GetComponent<EnemyReferences>();
        stateMachine = new StateMachine();

        //STATES
        var idle = new EnemyState_Idle(enemyReferences);
        var chase = new EnemyState_Chase(enemyReferences);
        var combat = new EnemyState_Combat(enemyReferences); 
        var delay = new EnemyState_Delay(2f);

        //TRANSITIONS
        At(idle, chase, () => OnPlayerDetected);
        At(chase, idle, () => !OnPlayerDetected);

        //START STATE
        stateMachine.SetState(idle);

        //FUNCTIONS & CONDITIONS
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to,Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
    }

    private void Update()
    {
        stateMachine.Tick();
    }
    private void FixedUpdate()
    {
        OnPlayerDetected = OnPlayerClose();
    }

    private void OnDrawGizmos()
    {
        if (stateMachine != null)
        {
            Gizmos.color = stateMachine.GetGizmoColor();
            Gizmos.DrawSphere(transform.position + Vector3.up * 3, 0.4f);
        }
    }
    //Move this to its own component and then add a ref to the EnemyReferences.class
    private bool playerInsideTrigger = false;
    [SerializeField] private LayerMask layerMask;
    private Transform detectedPlayer;
    public bool OnPlayerDetected { get; private set; } = false;
    public bool OnPlayerClose()
    {
        if (!playerInsideTrigger) return false;
        else
        {
            //Make sure to also include line of sight mwaybe? Using the dotP
            RaycastHit hit;
            //This could be just one check:
            //if hits anything
            if (Physics.Raycast(transform.position, (detectedPlayer.position - transform.position).normalized, out hit, Mathf.Infinity) && (hit.transform.gameObject.tag == "Player"))
            {
                //if hits object tagged with "Player"
                if (hit.transform.gameObject.tag == "Player")
                {
                    Debug.DrawRay(transform.position, (detectedPlayer.position - transform.position).normalized * hit.distance, Color.yellow);
                    Debug.Log("Player Hit");
                    return true;
                }
            }
            //if hit nothing:
            
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("No line of sight");            
            return false;


        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            detectedPlayer = other.transform;
            playerInsideTrigger = true;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //detectedPlayer = null; //Not sure if im optimizing anything by removing the reference
            playerInsideTrigger = false;
        }
    }

}
