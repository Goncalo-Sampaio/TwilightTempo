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
    private AudioSource healthSounds;
    [SerializeField] private AudioClip[] takingDamageSFX;
    [SerializeField] private AudioClip hitImpactSFX;
    public bool invunerable = false;
    void Start()
    {
        invunerable = false;
        healthSounds = GetComponent<AudioSource>();
        damageScreenController = gameObject.GetComponent<DamageScreenController>();
        flash = gameObject.GetComponent<Flash>();
        currentHealth = maxHealth;
        FindAnyObjectByType<UIManager>().SetHealth(maxHealth, currentHealth);
        if (LevelDataManager.Instance != null) LevelDataManager.Instance.AddPlayer(this);
        else Debug.LogWarning("LevelDataManager is missing - Add one to the scene");    
    }

    private void SetupUIReferences()
    {
        healthUI = LevelDataManager.Instance.playerCanvas.healthUI;
        if (LinkToHealthUi)
        {
            healthUI.maxValue = maxHealth;
            healthUI.value = currentHealth;
        }
    }
    private void OnEnable()
    {
        LevelDataManager.onCanvasRegister += SetupUIReferences;
    }
    private void OnDisable()
    {
        LevelDataManager.onCanvasRegister -= SetupUIReferences;
    }

    private void PlayGotDamageSound()
    {
        PlaySound(takingDamageSFX[(int)UnityEngine.Random.Range(0, takingDamageSFX.Length)]);
    }
    private void PlaySound(AudioClip clip)
    {

        healthSounds.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        healthSounds.PlayOneShot(clip);
        healthSounds.PlayOneShot(hitImpactSFX,0.3f);
    }

    public void Damage()
    {
        if (invunerable) return;
        Damage(5f);
        PlayGotDamageSound();
    }
    public void Damage(float damage)
    {
        if (invunerable) return;
        Damage(new Vector3(0f, -0.5f, -1f), damage);        
    }

    //We need a version of this with the int/ enum of the attack to check against already registered attacks
    //Force comes from collision contact point -> change this on enemy
    public void Damage(Vector3 force ,float damage)
    {
        if (invunerable) return;
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            //Destroy(gameObject);
            if (LinkToHealthUi)healthUI.value = 0;
            FindAnyObjectByType<UIManager>().ActivateScreen(1);
        }
        else
        {
            //Should be moved to its own class:
            if (LinkToHealthUi)healthUI.value = currentHealth;
            PlayGotDamageSound();
        }

        //VISUAL FEEDBACK:
        //Screenshake + Damage Overlay:
        damageScreenController.ScreenDamageEffect(screenShakeIntensityTestin);
        //Flash once
        flash.FlashForXIterations(1);
        //Reduce health
    }
    public void FetchMeTheirSouls(int healValue)
    {
        //trigger a vfx on player
        gameObject.GetComponent<Flash>().HealVisual();
        if (currentHealth + healValue >= maxHealth) currentHealth = maxHealth;
        else currentHealth += healValue;
        if (LinkToHealthUi) healthUI.value = currentHealth;
        Debug.Log("Player healed, new health = " + currentHealth);
    }
    public void Heal()
    {
        currentHealth = maxHealth;
        if (LinkToHealthUi) healthUI.value = currentHealth;
    }
    
}
