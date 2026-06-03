using System.Collections;
using UnityEngine;

public class TeleportFeedback : MonoBehaviour
{
    private Animator animator;
    [SerializeField]private CanvasGroup TextPromptGroup;
    private float textAlpha = 0f;
    [SerializeField] private float textFadeSpeed = 0.1f;
    public bool fade = false;
    private TeleportCrystals[] crystals;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        
    }
    private void Start()
    {
        crystals = GetComponentsInChildren<TeleportCrystals>();
    }
    public void ToggleAnimation(bool on) => animator.SetBool("Pulse", on);
    private void Update()
    {
        if (!fade)
        {
            if (textAlpha <= 1f)
            {
                textAlpha += textFadeSpeed * Time.deltaTime;
                TextPromptGroup.alpha = textAlpha;
            }
        }
        else
        {
            if (textAlpha > 0f)
            {
                textAlpha -= textFadeSpeed * 10 * Time.deltaTime;
                TextPromptGroup.alpha = textAlpha;
            }
        }
        
    }
    private bool playerInRangeOfCrystals = false;
    private void FixedUpdate()
    {
        playerInRangeOfCrystals = crystals[0].playerInRange || crystals[1].playerInRange;
        if (playerInRangeOfCrystals)
        {
            ToggleAnimation(true);
            fade = false;
        }
        else
        {
            ToggleAnimation(false);
            fade = true;
        }
    } 

}