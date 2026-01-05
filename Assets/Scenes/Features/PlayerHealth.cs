using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 200;
    [SerializeField]
    private float currentHealth;
    [SerializeField]
    private Slider healthUI;

    void Start()
    {

        currentHealth = maxHealth;
        healthUI.maxValue = maxHealth;
        healthUI.value = currentHealth;
    }

    public void Damage()
    {
        currentHealth -= 5;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            healthUI.value = 0;
            SceneManager.LoadScene(0);
        }
        else
        {
            healthUI.value = currentHealth;
        }
    }

    public void Heal()
    {
        currentHealth = maxHealth;
        healthUI.value = currentHealth;
    }
    
}
