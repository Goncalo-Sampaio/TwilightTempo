using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SkillSlot : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject skillTimer;
    private TextMeshProUGUI skillTimerText;
    private PlayerStateManagerPlayables playerStateManagerPlayables;
    private PlayerStates currentState;

    private SkillSO skillSO;

    private string skillName;
    private SkillType skillType;
    private SkillAttunement skillAttunement;
    private float cooldown;
    private Sprite icon;
    private GameObject skillObject;
    private AnimatorOverrideController skillAOV;
    private ISkill skillScript;
    private AnimationClip skillAnimation;

    private float skillTime;
    private float castTime;
    private float currentCooldown;
    private float animationTimer;
    private bool exitSkill = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateManagerPlayables = GetComponentInParent<PlayerStateManagerPlayables>();
        currentState = playerStateManagerPlayables.CurrentState;
        skillTimerText = skillTimer.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void FixedUpdate()
    {
        currentState = playerStateManagerPlayables.CurrentState;

        currentCooldown -= Time.fixedDeltaTime;
        animationTimer -= Time.fixedDeltaTime;

        if (currentCooldown < 0)
        {
            currentCooldown = 0;
            skillTimer.gameObject.SetActive(false);
            skillTimerText.gameObject.SetActive(false);
        }
        else
        {
            skillTimerText.text = TimeSpan.FromSeconds(currentCooldown).ToString("ss").TrimStart('0');
            skillTimer.gameObject.SetActive(true);
            skillTimerText.gameObject.SetActive(true);
        }

        if (animationTimer < 0)
        {
            animationTimer = 0;

            if (exitSkill && currentState == PlayerStates.Skill)
            {
                playerStateManagerPlayables.ResetState();
            }

            exitSkill = false;
        }
    }

    public void AssignSkill(SkillSO skill)
    {
        skillSO = skill;
        skillName = skill.skillName;
        skillType = skill.skillType;
        skillAttunement = skill.skillAttunement;
        cooldown = skill.cooldown;
        icon = skill.icon;
        skillAnimation = skill.animation;

        skillObject = Instantiate(skill.skillObject, this.transform);

        skillScript = skillObject.GetComponent<ISkill>();

        //skillAOV = skill.skillAnimatorOverride;
    }

    public void ActivateSlot()
    {
        if (currentCooldown <= 0)
        {
            skillScript.Cast();
            currentCooldown = cooldown;

            //First attempt
            //animator.Play("Skill", 0, 0);

            //Second attempo (Animator Override)
            //animator.runtimeAnimatorController = skillAOV;
            //animator.CrossFadeInFixedTime("Skill", 0.25f, 0, 0.0f, 0.0f);

            //Playables
            animationTimer = skillAnimation.length;
            playerStateManagerPlayables.SetCurrentState(PlayerStates.Skill);
            playerStateManagerPlayables.Attack(skillAnimation);
            exitSkill = true;
        }
    }
}
