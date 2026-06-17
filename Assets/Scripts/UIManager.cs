using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{    
    [SerializeField]
    private GameObject finisherReady;
    [Header("Referenceable UI elements:")]
    [SerializeField] private Slider gauge;
    [SerializeField] private GameObject skillTimer1;
    [SerializeField] private GameObject skillTimer2;
    [SerializeField] private GameObject skillTimer3;
    [SerializeField] private GameObject skillTimer4;
    [SerializeField] private GameObject skillTimer5;
    [SerializeField] private GameObject skillTimer6;
    public GameObject skillHolder;
    public Slider healthUI;

    [SerializeField]
    private GameObject enemyHealth;

    [SerializeField]
    private TextMeshProUGUI spacebarPrompt;
    [SerializeField]
    private GameObject[] screens;
    [SerializeField]
    private float timeToFade = 1f;
    [SerializeField]
    private float textAlpha = 0f;

    [SerializeField]
    private float screensCounter = 0f;
    [SerializeField]
    private bool counting = false;
    [SerializeField]
    private bool canInteractWithScreen = false;

    [SerializeField]
    private MovementPlayables playerMov;
    [SerializeField]
    private PlayerCombatPlayables playerAttack;
    [SerializeField]
    private SkillSystem playerSkills;
    [SerializeField]
    private PlayerDodge playerDodge;
    [SerializeField]
    private Teleport playerTeleport;
    private void Awake()
    {
        var canvasOBJ = GetComponent<Canvas>();
        if (!canvasOBJ.isActiveAndEnabled) canvasOBJ.enabled = true;
    }
    private void Start()
    {
        ActivateScreen(0);

        if (LevelDataManager.Instance != null)
        {
            LevelDataManager.Instance.RegisterCanvas(this);

        }
        else Debug.LogWarning("LevelDataManager is missing - Add one to the scene");
    }

    private void Update()
    {
        if (textAlpha <= 1f && canInteractWithScreen)
        {
            textAlpha +=  Time.unscaledDeltaTime/timeToFade;
            spacebarPrompt.alpha = textAlpha;
        }

        if (screensCounter > 0f)
        {
            screensCounter -= Time.unscaledDeltaTime;
        }

        if (screensCounter <= 0f && counting)
        {
            counting = false;
            canInteractWithScreen = true;
        }

        if (Input.GetKey(KeyCode.Space) && canInteractWithScreen)
        {
            textAlpha = 0f;
            spacebarPrompt.alpha = 0f;
            canInteractWithScreen = false;
            Time.timeScale = 1f;

            DisablePlayerActions(false);

            if (screens[0].activeInHierarchy)
            {
                screens[0].SetActive(false);
            }
            else if (screens[1].activeInHierarchy)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else if (screens[2].activeInHierarchy)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    public void ActivateFinisher(bool activated)
    {
        finisherReady.SetActive(activated);
    }

    public void ChangeGauge(float gaugeValue)
    {
        gauge.value = gaugeValue;
    }
    
    public GameObject GetSkillVisual(int skillNumber)
    {
        if (skillNumber == 1) return skillTimer1;
        else if (skillNumber == 2) return skillTimer2;
        else if (skillNumber == 3) return skillTimer3;
        else if (skillNumber == 4) return skillTimer4;
        else if (skillNumber == 5) return skillTimer5;
        else if (skillNumber == 6) return skillTimer6;
        else
        {
            Debug.Log($"No Skills with {skillNumber} skill Number");
            return null;
        }
    }

    public void UpdateEnemyHealth(bool showHealth, float maxHealth, float currentHealth)
    {
        if (showHealth)
        {
            enemyHealth.GetComponentInChildren<Slider>().maxValue = maxHealth;
            enemyHealth.GetComponentInChildren<Slider>().value = currentHealth;
            enemyHealth.SetActive(true);
        }
        else
        {
            enemyHealth.SetActive(false);
        }
    }

    public void SetHealth(float maxHealth, float currentHealth)
    {
        healthUI.maxValue = maxHealth;
        healthUI.value = currentHealth;
    }

    /// <summary>
    /// activate the corresponding screen
    /// 0 - intro, 1 - game over, 2 - end screen
    /// </summary>
    /// <param name="screenId"></param>
    public void ActivateScreen(int screenId)
    {
        DisablePlayerActions(true);
        Time.timeScale = 0f;
        screens[screenId].SetActive(true);
        screensCounter = 1f;
        counting = true;
    }

    public void DisablePlayerActions(bool enable)
    {
        playerMov.Paused = enable;
        playerAttack.Paused = enable;
        playerSkills.Paused = enable;
        playerDodge.Paused = enable;
        playerTeleport.Paused = enable;
    }
}
