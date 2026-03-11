using NaughtyAttributes;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    [Button]
    public void Hit() => _animator.SetTrigger("Hit");
    [Button]
    public void Die() => _animator.SetTrigger("Die");
    [Button]
    public void Attack1() => _animator.SetTrigger("Attack1");
    [Button]
    public void Attack2() => _animator.SetTrigger("Attack2");
    [Button]
    public void Attack3() => _animator.SetTrigger("Attack3");
    [Button]
    public void StartIdle() => _animator.SetBool("Idle", true);
    [Button]
    public void StopIdle() => _animator.SetBool("Idle", false);
    [Button]
    public void StartWalking() => _animator.SetBool("Walking", true);
    [Button]
    public void StopWalking() => _animator.SetBool("Walking", false);
    [Button]
    public void StartRunning() => _animator.SetBool("Running", true);
    [Button]
    public void StopRunning() => _animator.SetBool("Running", false);
    public void WarCry() => _animator.SetTrigger("WarCry");
    public void Berserk(float berserskAnimationSpeed = 1.5f) => _animator.speed = berserskAnimationSpeed;
}
