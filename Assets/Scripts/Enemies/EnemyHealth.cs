using DG.Tweening;
using DG.Tweening.Core.Easing;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

public class EnemyHealth : MonoBehaviour
{
    //Make a base health class so i can stop copying code around 
    [SerializeField] private LayerMask playerDamageLayer;
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [SerializeField][Tooltip("Percentage Based - how much knockback is negated?")] private float knockBackResistance = 10f;
    public bool dead;
    
    
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip magicHitSFX;

    private EnemyBrain brain;
    
    private AudioSource audioSource;
    private EnemyReferences enemyReferences;
    private ProgressionBlocker progressionBlocker;
    private Flash flash;
    private bool wasHit;
    private bool wasMiss;
    private bool gettingKnockBacked = false;
    [SerializeField] private float maxKnockBackTime = 3f;
    [SerializeField] private float staggerTimmer = .75f;    
    
    public void SetProgressionBlocker(ProgressionBlocker progressionBlocker) => this.progressionBlocker = progressionBlocker;
    void Start()
    {
        enemyReferences = GetComponent<EnemyReferences>();
        brain = enemyReferences.enemyBrain;
        flash = enemyReferences.flash;        
        currentHealth = maxHealth;


        audioSource = GetComponent<AudioSource>();
        dead = false;
    }
    //Just Damage
    public void Damage(float damage)
    {
        PlayGettingHitSounds();

        currentHealth -= damage;        
        

        if (currentHealth <= 0)
        {
            if (progressionBlocker != null)
            {
                progressionBlocker.RemoveEnemy(this);
            }

            if (!dead)
            {
                StartCoroutine(DeathRot());                
            }

        }
        //VISUAL FEEDBACK:
        //Flash once
        GotHit();
        
    }
    //With KnockBack
    public void Damage(float damage,Vector3 force)
    {
        PlayGettingHitSounds();
        currentHealth -= damage;
        Vector3 forceAfterKnockBackNegation = force - (force * knockBackResistance / 100 );
        if (currentHealth - damage <= 0 ) ApllyKnockBack(forceAfterKnockBackNegation);

        if (currentHealth <= 0)
        {
            if (progressionBlocker != null)
            {
                progressionBlocker.RemoveEnemy(this);
            }

            if (!dead)
            {
                StartCoroutine(DeathRot());
                brain.dead = true;
            }
            
        }
        //VISUAL FEEDBACK:
        //Flash once
        GotHit();
        //flash.FlashForXIterations(1);
        
        //transform.DOShakePosition(0.2f, 0.1f, 10);

    }
    public void GotHit()
    {
        if (!wasHit) StartCoroutine(GotHitRot());
    }
    
    [Button]
    public void ApllyKnockBack() => ApllyKnockBack(10f * -transform.forward + transform.up);
    public void ApllyKnockBack(Vector3 force) => StartCoroutine("ApplyKnockBackRot", force);
    private IEnumerator ApplyKnockBackRot(Vector3 force)
    {
        gettingKnockBacked = true;

        yield return null; //wait one frame to make sure all courotines are stopped
        //Only call agent.Stop if:
        //  agent is active
        //  agent is on NavMesh;
        //  agent isint' already stopped

        enemyReferences.enemyNavigation.StopNow(true);
        enemyReferences.enemyNavigation.ToggleEnableAgent(false); //disable agent

        enemyReferences.rb.useGravity = true;
        enemyReferences.rb.isKinematic = false;
        enemyReferences.rb.AddForce(force, ForceMode.Impulse);


        //only exit after the fixedUpdate frame is passed. To make sure the force is applied
        yield return new WaitForFixedUpdate();
        float knockBackTime = Time.time;

        yield return new WaitUntil(() => enemyReferences.rb.linearVelocity.magnitude < 0.05f || Time.time > knockBackTime + maxKnockBackTime); //wait until it stops moving.

        //
        yield return new WaitForSeconds(0.25f); //stun frames //consider adding a flash here

        //now reset:
        enemyReferences.rb.linearVelocity = Vector3.zero;
        enemyReferences.rb.angularVelocity = Vector3.zero;
        enemyReferences.rb.useGravity = false;
        enemyReferences.rb.isKinematic = true;

        //snap agent back to navmesh
        enemyReferences.enemyNavigation.Warp(transform.position);
        //THEN AND ONLY THEN
        //enable the agent
        enemyReferences.enemyNavigation.ToggleEnableAgent(true); //enable agent
        enemyReferences.enemyNavigation.ToggleAgentStopped(false); //start agent navmesh

        gettingKnockBacked = false;
    }
    private IEnumerator GotHitRot()
    {
        wasHit = true;
        yield return new WaitForSeconds(staggerTimmer);
        wasHit = false;
    }
    private IEnumerator DeathRot()
    {
        dead = true;
        enemyReferences.enemyNavigation.StopNow(true);
        enemyReferences.enemyAnimator.Die();
        enemyReferences.enemyBrain.dead = true;
        yield return new WaitForSeconds(3);        
        Destroy(this.gameObject);

    }
    private void PlayGettingHitSounds()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(hitSFX);
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(magicHitSFX);
    }

}
