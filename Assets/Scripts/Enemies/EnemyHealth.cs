using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;
    [SerializeField]
    private float currentHealth;

    [SerializeField]
    private ProgressionBlocker progressionBlocker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;

        if (progressionBlocker != null)
        {
            progressionBlocker.RegisterEnemy(this);
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
}
