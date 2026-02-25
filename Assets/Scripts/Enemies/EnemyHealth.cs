using DG.Tweening;
using DG.Tweening.Core.Easing;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
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

    private EnemyBrain brain;
    public Transform player;

    private Flash flash;

    void Start()
    {
        brain = GetComponent<EnemyBrain>();
        flash = gameObject.GetComponent<Flash>();
        dead = false;
        currentHealth = maxHealth;

        if (progressionBlocker != null)
        {
            progressionBlocker.RegisterEnemy(this);
        }
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
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if (progressionBlocker != null)
            {
                progressionBlocker.RemoveEnemy(this);
            }

            Destroy(gameObject);
            
        }
        //VISUAL FEEDBACK:
        //Flash once
        flash.FlashForXIterations(1);
        brain.KnockTest((transform.position - player.position +transform.up * .4f).normalized * knockBackTaken);
        //transform.DOShakePosition(0.2f, 0.1f, 10);

    }
    public void Die()
    {
        dead = true;
        Invoke("Destroy",3f);
    }
}
