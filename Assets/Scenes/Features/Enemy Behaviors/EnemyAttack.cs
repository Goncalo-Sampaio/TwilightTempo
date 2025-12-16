using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private Animator attackAnimator;

    public void AttackPlayer()
    {
        //Also check for collissions and call TakeDamage() on player if contact
        attackAnimator.SetTrigger("Attack");
    }
}
