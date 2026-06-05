using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] 
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject pauseMenuBackground;
    [SerializeField]
    private GameObject settingsMenu;
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private float musicValue;
    [SerializeField]
    private float sfxValue;
    [SerializeField]
    private TextMeshProUGUI musicValueTMP;
    [SerializeField]
    private TextMeshProUGUI sfxValueTMP;
    [SerializeField]
    private GameObject confirmationPopup;

    [Header("PlayerPrefs")]
    [SerializeField]
    private string PlayerPrefsVolumeKey = "MasterVolumeValue";
    [SerializeField]
    private string PlayerPrefsSFXKey = "MasterSFXValue";

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

    private bool isPaused = false;

    public bool CanPause { get; set; } = true;

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(PlayerPrefsVolumeKey, 1f);
        float savedSFX = PlayerPrefs.GetFloat(PlayerPrefsSFXKey, 1f);

        musicValue = savedVolume;
        sfxValue = savedSFX;

        ChangeMusicVolume(0f);
        ChangeSFXVolume(0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CanPause)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else if (!isPaused)
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
        DisablePlayerActions(true);
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        pauseMenuBackground.SetActive(true);
    }

    public void OpenSettings(bool open)
    {
        settingsMenu.SetActive(open);
        pauseMenu.SetActive(!open);
    }
    public void OpenConfirmation(bool open)
    {
        confirmationPopup.SetActive(open);
        pauseMenu.SetActive(!open);
    }

    public void DisablePlayerActions(bool enable)
    {
        playerMov.Paused = enable;
        playerAttack.Paused = enable;
        playerSkills.Paused = enable;
        playerDodge.Paused = enable;
        playerTeleport.Paused = enable;
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenu.SetActive(false);
        pauseMenuBackground.SetActive(false);
        settingsMenu.SetActive(false);
        isPaused = false;
        DisablePlayerActions(false);
        Time.timeScale = 1f;
    }

    public void ReturnToMainMenu()
    {
        ResumeGame(); // ensure unpaused
        SceneManager.LoadScene(0);

        /*if (levelManager != null)
        {
            levelManager.ResetToMainMenu(); // stop level, deactivate isRunning, reset all
        }

        if (mainMenuState != null)
        {
            mainMenuState.RestoreState(); // reactivate main menu civillians
        }

        if (gameUI != null)
            gameUI.SetActive(false); // deactivate player UI*/
    }

    private void UpdateHandleSprite(float value, Image handleImage, Sprite mute, Sprite low, Sprite mid, Sprite high)
    {
        if (value <= 0.10f)
            handleImage.sprite = mute;
        else if (value <= 0.43f)
            handleImage.sprite = low;
        else if (value <= 0.76f)
            handleImage.sprite = mid;
        else
            handleImage.sprite = high;
    }
    public void ChangeMusicVolume(float change)
    {
        float value = musicValue + change;

        // Protect against log(0)
        if (value <= 0.0001f) value = 0.0001f;
        if (value >= 1f) value = 1f;

        musicValue = value;

        float dB = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);

        PlayerPrefs.SetFloat(PlayerPrefsVolumeKey, value);

        musicValueTMP.text = (Mathf.Round(musicValue * 100)).ToString() + "%";
    }

    public void ChangeSFXVolume(float change)
    {
        float value = sfxValue + change;

        // Protect against log(0)
        if (value <= 0.0001f) value = 0.0001f;
        if (value >= 1f) value = 1f;

        sfxValue = value;

        float dB = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("SFXVolume", dB);

        PlayerPrefs.SetFloat(PlayerPrefsSFXKey, value);

        sfxValueTMP.text = (Mathf.Round(sfxValue * 100)).ToString() + "%";
    }
}
