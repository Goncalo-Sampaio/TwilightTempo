using DG.Tweening;
using DG.Tweening.Core.Easing;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

public class EnemyHealth : MonoBehaviour
{
    //Make a base health class so i can stop copying code around 
    [SerializeField]
    private LayerMask playerDamageLayer;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float knockBackTaken = 5f;
    public bool dead;

    [SerializeField]
    private ProgressionBlocker progressionBlocker;
    [SerializeField]
    private AudioClip hitSFX;
    [SerializeField]
    private AudioClip magicHitSFX;

    private EnemyBrain brain;
    public Transform player;
    private AudioSource audioSource;
    private EnemyReferences enemyReferences;

    private Flash flash;

    void Start()
    {
        enemyReferences = GetComponent<EnemyReferences>();
        brain = GetComponent<EnemyBrain>();
        flash = gameObject.GetComponent<Flash>();
        dead = false;
        currentHealth = maxHealth;

        if (progressionBlocker != null)
        {
            progressionBlocker.RegisterEnemy(this);
        }

        audioSource = GetComponent<AudioSource>();
    }
    [Button]

    public void Damage()
    {
        Damage(35f);
    }
    //Damage should be dealt from player
    //private void OnTriggerEnter(Collider other)
    //{
    //    if ((playerDamageLayer.value & (1 << other.transform.gameObject.layer)) > 0)
    //    {
    //        //brain.KnockTest((transform.position - player.position +transform.up * .4f).normalized * 100f);
    //        Damage(1f);      
    //    }
    //}

    public void Damage(float damage)
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(hitSFX);
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(magicHitSFX);
        currentHealth -= damage;

        if (currentHealth - damage <= 0 ) brain.KnockTest((transform.position - player.position + transform.up * .4f).normalized * knockBackTaken);

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
        brain.GotHit();
        //flash.FlashForXIterations(1);
        
        //transform.DOShakePosition(0.2f, 0.1f, 10);

    }

    private IEnumerator DeathRot()
    {
        dead = true;
        enemyReferences.enemyNavigation.StopNow(true);
        enemyReferences.enemyAnimator.Die();
        yield return new WaitForSeconds(3);        
        Destroy(this.gameObject);

    }


}
