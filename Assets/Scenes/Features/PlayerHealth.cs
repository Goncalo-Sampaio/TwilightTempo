using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 200;
    [SerializeField]
    private float currentHealth;
    void Start()
    {

        currentHealth = maxHealth;

    }

    public void Damage()
    {
        currentHealth -= 10;

        if (currentHealth <= 0)
        {

            Destroy(gameObject);


        }
    }
    
}
