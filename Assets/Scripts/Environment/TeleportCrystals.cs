using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportCrystals : MonoBehaviour
{
    [SerializeField]
    private GameObject targetCrystal;
    [SerializeField]
    private float interactionRange = 5f;

    private Teleport player;

    [SerializeField] private Collider crystalMeshCollider;
    [SerializeField] private Collider[] bridgeMeshColliders;
    [SerializeField] private SphereCollider playerTriggerColl;
    private bool enableColliderRotRunning;
    public bool playerInRange;
    public static bool  wasTeleporting ;
    [SerializeField] private float delayBeforeEnablingColliders = 1f;
    private TeleportFeedback teleportFeedback;
    void Start()
    {
        player = FindAnyObjectByType<Teleport>();
        teleportFeedback = GetComponentInParent<TeleportFeedback>();
        enableColliderRotRunning = false;
    }
    private void OnValidate()
    {
        playerTriggerColl.radius = interactionRange;
    }
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.T))
        {
            player.ActivateTeleport(targetCrystal);
            wasTeleporting = true;
        }

    }
    private void FixedUpdate()
    {
        if (playerInRange)
        {
            //if player is in range and also teleportin
            if (player.teleporting)
            {
                crystalMeshCollider.isTrigger = true;                
                foreach (Collider collider in bridgeMeshColliders) { collider.isTrigger = true; }
            }
        }
        //triggered only was previously teleporting and then not (teleporting >> !teleporting)
        //only call coroutine on that state transition
        if (wasTeleporting)
        {
            if (!enableColliderRotRunning) StartCoroutine(DelayedEnableMeshCollider());
            wasTeleporting = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) playerInRange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) playerInRange = false;
        
    }
    private IEnumerator DelayedEnableMeshCollider()
    {
        enableColliderRotRunning = true;
        yield return new WaitForSeconds(delayBeforeEnablingColliders);
        yield return new WaitForFixedUpdate();
        crystalMeshCollider.isTrigger = false;
        foreach (Collider collider in bridgeMeshColliders) { collider.isTrigger = false; }
        enableColliderRotRunning =false;
    }
    
}
