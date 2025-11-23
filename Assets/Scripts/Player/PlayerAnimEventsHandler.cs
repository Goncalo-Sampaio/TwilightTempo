using UnityEngine;

public class PlayerAnimEventsHandler : MonoBehaviour
{
    [SerializeField]
    private Collider playerDamageCollider;

    private PlayerStateManagerPlayables stateManagerPlayables;
    private PlayerCombatPlayables playerCombatPlayables;

    private void Start()
    {
        stateManagerPlayables = GetComponentInParent<PlayerStateManagerPlayables>();
        playerCombatPlayables = GetComponentInParent<PlayerCombatPlayables>();
    }

    public void ActivateWeapon()
    {
        playerDamageCollider.enabled = true;
        playerCombatPlayables.EnableCombo();
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
