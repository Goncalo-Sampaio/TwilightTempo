using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    
    [SerializeField] private GameObject[] meshes;
    
    [SerializeField] private float timeToShow = 1f;
    
    [SerializeField] private ParticleSystem spawnParticles;
    [SerializeField] private float attackSpeed = .5f;
    [SerializeField] private float baseSpeed = 1.0f;
    [SerializeField] private float berserskAnimationSpeedMultiplier = 1.5f;

    private Animator _animator;
    private bool hitStopActive = false;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }   

    public void EnableSpawn()
    {
        spawnParticles.Play();
        StartCoroutine(AppearCoroutine());
    }

    [Button]
    public void Hit()
    {
        UpdateSpeed();
        _animator.SetTrigger("Hit");
    } 
    [Button]
    public void Die()
    {
        UpdateSpeed();
        _animator.SetTrigger("Die");

    }
    public void HitStop(int frames = 10)
    {
        if (!hitStopActive) StartCoroutine(HitStopRot(frames));
    }
    private IEnumerator HitStopRot(int frames)
    {
        _animator.enabled = false;
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
        _animator.enabled = true;
    }
    [Button]
    public void Attack1()
    {
        UpdateSpeed();
        _animator.SetTrigger("Attack1");
    }
    [Button]
    public void Attack2()
    {
        UpdateSpeed();
        _animator.SetTrigger("Attack2");
    }
    [Button]
    public void Attack3()
    {
        UpdateSpeed();
        _animator.SetTrigger("Attack3");
    }
    [Button]
    public void StartIdle()
    {
        UpdateSpeed();
        _animator.SetFloat("IdleOffset", Random.Range(0f, 1f));
        _animator.SetBool("Idle", true);
        
    }
    [Button]
    public void StopIdle()
    {
        UpdateSpeed();
        _animator.SetBool("Idle", false);
    } 
    [Button]
    public void StartWalking()
    {
        UpdateSpeed();
        _animator.SetBool("Walking", true);
    }
    [Button]
    public void StopWalking()
    {
        UpdateSpeed();
        _animator.SetBool("Walking", false);
    }
    [Button]
    public void StartRunning()
    {
        UpdateSpeed();
        _animator.SetBool("Running", true);
    } 
    [Button]
    public void StopRunning()
    {
        UpdateSpeed();
        _animator.SetBool("Running", false);
    }
    [Button]
    public void WarCry()
    {
        UpdateSpeed();
        _animator.SetTrigger("WarCry");
    }
    [Button]
    public void Casting()
    {
        UpdateSpeed();
        _animator.SetTrigger("CastingSpell");
    }
    
    [Button]
    public void SpellCast()
    {
        UpdateSpeed();
            _animator.SetTrigger("Spell");
    }
    public void Berserk() => _animator.speed = baseSpeed * berserskAnimationSpeedMultiplier;

    private IEnumerator AppearCoroutine()
    {
        yield return new WaitForSeconds(timeToShow);
        foreach (GameObject mesh in meshes)
        {
            mesh.SetActive(true);
        }
    }
    private void UpdateSpeed() => _animator.speed = (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1")) ? attackSpeed : baseSpeed;
}
