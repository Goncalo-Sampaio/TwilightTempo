using DG.Tweening.Plugins.Options;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    private float teleportSpeed;
    [SerializeField]
    private float stopRange;
    [SerializeField]
    private GameObject annikaModel;
    [SerializeField]
    private GameObject teleportObject;

    private GameObject targetCrystal;

    private Vector3 direction;
    public bool teleporting = false;
    private Rigidbody rb;
    private PlayerStateManagerPlayables stateManager;
    private float stopBufferRange = 2f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        stateManager = GetComponent<PlayerStateManagerPlayables>();
        teleportLerpedSpeed = teleportSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        if (teleporting)
        {
            if (Vector3.Distance(targetCrystal.transform.position, transform.position) < stopRange)
            {
                annikaModel.SetActive(true);
                teleportObject.SetActive(false);
                teleporting = false;
                stateManager.ResetState();
                //reset teleport speed:
                teleportLerpedSpeed = teleportSpeed;
            }
        }
    }
    private float teleportLerpedSpeed = 0;
    private void FixedUpdate()
    {
        if (teleporting)
        {
            //Start rapidly decreaseing teleport speed the closer the player is to the endposition (Starting "stopBufferRange" from stopping distance:)
            if (Vector3.Distance(targetCrystal.transform.position, transform.position) < stopRange + stopBufferRange)
            {
                teleportLerpedSpeed = teleportSpeed / Mathf.Max((stopRange + stopBufferRange - Vector3.Distance(targetCrystal.transform.position, transform.position)), 1);
            }            
             rb.linearVelocity = direction.normalized * teleportLerpedSpeed;
        }
    }

    public void ActivateTeleport(GameObject target)
    {
        annikaModel.SetActive(false);
        teleportObject.SetActive(true);
        targetCrystal = target;
        rb.linearVelocity = Vector3.zero;
        stateManager.SetCurrentState(PlayerStates.Teleporting);
        direction = target.transform.position - transform.position;        
        teleporting = true;
    }
}
