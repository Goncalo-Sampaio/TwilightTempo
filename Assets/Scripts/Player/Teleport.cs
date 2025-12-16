using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    private float teleportSpeed;
    [SerializeField]
    private float stopRange;

    private GameObject targetCrystal;

    private Vector3 direction;
    private bool teleporting = false;
    private Rigidbody rb;
    private PlayerStateManagerPlayables stateManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        stateManager = GetComponent<PlayerStateManagerPlayables>();
    }

    // Update is called once per frame
    void Update()
    {
        if (teleporting)
        {
            if (Vector3.Distance(targetCrystal.transform.position, transform.position) < stopRange)
            {
                teleporting = false;
                stateManager.ResetState();
            }
        }
    }

    private void FixedUpdate()
    {
        if (teleporting)
        {
            rb.linearVelocity = direction.normalized * teleportSpeed;
        }
    }

    public void ActivateTeleport(GameObject target)
    {
        
        targetCrystal = target;
        rb.linearVelocity = Vector3.zero;
        stateManager.SetCurrentState(PlayerStates.Teleporting);
        direction = target.transform.position - transform.position;
        teleporting = true;
    }
}
