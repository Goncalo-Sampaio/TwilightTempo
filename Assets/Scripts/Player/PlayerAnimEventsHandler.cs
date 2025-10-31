using UnityEngine;

public class PlayerAnimEventsHandler : MonoBehaviour
{
    [SerializeField]
    private Collider playerDamageCollider;

    private PlayerStateManager stateManager;

    private void Start()
    {
        stateManager = GetComponentInParent<PlayerStateManager>();
    }

    public void ActivateWeapon()
    {
        playerDamageCollider.enabled = true;
    }

    public void DeactivateWeapon()
    {
        playerDamageCollider.enabled = false;
    }

    public void ActivateMovement()
    {
        stateManager.ResetState();
    }
}
