using DG.Tweening.Core.Easing;
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

    private DamageScreenController damageScreenController;
    [SerializeField] private float screenShakeIntensityTestin = 0.6f;

    private Flash flash;
    [SerializeField] private bool LinkToHealthUi = true;
    void Start()
    {
        damageScreenController = gameObject.GetComponent<DamageScreenController>();
        flash = gameObject.GetComponent<Flash>();
        currentHealth = maxHealth;
        if (LinkToHealthUi)
        {
            healthUI.maxValue = maxHealth;
            healthUI.value = currentHealth; 
        }
        
    }

    public void Damage()
    {
        Damage(5f);        
    }
    public void Damage(float damage)
    {
        Damage(new Vector3(0f, -0.5f, -1f), damage);
    }

    //We need a version of this with the int/ enum of the attack to check against already registered attacks
    //Force comes from collision contact point -> change this on enemy
    public void Damage(Vector3 force ,float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            if (LinkToHealthUi)healthUI.value = 0;
            SceneManager.LoadScene(0);
        }
        else
        {
            //Should be moved to its own class:
            if (LinkToHealthUi)healthUI.value = currentHealth;
        }

        //VISUAL FEEDBACK:
        //Screenshake + Damage Overlay:
        damageScreenController.ScreenDamageEffect(screenShakeIntensityTestin);
        //Flash once
        flash.FlashForXIterations(1);
        //Reduce health
    }

    public void Heal()
    {
        currentHealth = maxHealth;
        if (LinkToHealthUi) healthUI.value = currentHealth;
    }
    
}
