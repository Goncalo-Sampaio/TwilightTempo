using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private LayerMask playerDamageLayer;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    public bool dead;

    [SerializeField]
    private ProgressionBlocker progressionBlocker;

    private EnemyBrain brain;
    public Transform player;
    void Start()
    {
        brain = GetComponent<EnemyBrain>();
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
        currentHealth -= 50;

        if (currentHealth <= 0)
        {
            if (progressionBlocker != null)
            {
                progressionBlocker.RemoveEnemy(this);
            }
            Destroy(gameObject);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((playerDamageLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            brain.KnockTest((transform.position - player.position +transform.up * .4f).normalized * 100f);
            Damage(1f);      
        }
    }

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
    }
    public void Die()
    {
        dead = true;
        Invoke("Destroy",3f);
    }
}
