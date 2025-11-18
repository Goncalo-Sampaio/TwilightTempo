using UnityEngine;

public class PlayerAnimEventsHandler : MonoBehaviour
{
    [SerializeField]
    private Collider playerDamageCollider;

    private PlayerStateManagerPlayables stateManagerPlayables;

    private void Start()
    {
        stateManagerPlayables = GetComponentInParent<PlayerStateManagerPlayables>();
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
        stateManagerPlayables.ResetState();
    }
}
