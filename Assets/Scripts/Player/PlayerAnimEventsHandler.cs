using UnityEngine;

public class PlayerAnimEventsHandler : MonoBehaviour
{
    [SerializeField]
    private Collider playerDamageCollider;

    public void ActivateWeapon()
    {
        playerDamageCollider.enabled = true;
    }

    public void DeactivateWeapon()
    {
        playerDamageCollider.enabled = false;
    }
}
