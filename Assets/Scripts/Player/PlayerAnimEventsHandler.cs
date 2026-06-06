using UnityEngine;

public class PlayerAnimEventsHandler : MonoBehaviour
{
    [SerializeField]
    private Collider playerWeaponCollider;
    [SerializeField]
    private SphereCollider playerDeflectCollider;
    [SerializeField]
    private Collider playerWeaponCollider2;
    [SerializeField]
    private SphereCollider playerDeflectCollider2;

    private PlayerStateManagerPlayables stateManagerPlayables;
    private PlayerCombatPlayables playerCombatPlayables;

    private void Start()
    {
        stateManagerPlayables = GetComponentInParent<PlayerStateManagerPlayables>();
        playerCombatPlayables = GetComponentInParent<PlayerCombatPlayables>();
    }

    public void ActivateWeapon(int weapon)
    {
        if (weapon == 0)
        {
            playerWeaponCollider.enabled = true;
            playerDeflectCollider.enabled = true;
        }
        else if (weapon == 1)
        {
            playerWeaponCollider2.enabled = true;
            playerDeflectCollider2.enabled = true;
        }
        else if (weapon == 2)
        {
            playerWeaponCollider.enabled = true;
            playerWeaponCollider2.enabled = true;
            playerDeflectCollider.enabled = true;
            playerDeflectCollider2.enabled = true;
        }

        playerCombatPlayables.EnableCombo();
    }

    public void DeactivateWeapon(int weapon)
    {
        if (weapon == 0)
        {
            playerWeaponCollider.enabled = false;
            playerDeflectCollider.enabled = false;
        }
        else if (weapon == 1)
        {
            playerWeaponCollider2.enabled = false;
            playerDeflectCollider2.enabled = false;
        }
        else if (weapon == 2)
        {
            playerWeaponCollider.enabled = false;
            playerWeaponCollider2.enabled = false;
            playerDeflectCollider.enabled = false;
            playerDeflectCollider2.enabled = false;
        }
    }

    public void ActivateMovement()
    {
        stateManagerPlayables.ResetState();
    }
}
