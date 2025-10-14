using System.Security.Cryptography.X509Certificates;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SkillSlot : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    private PlayerStateManager playerStateManager;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateManager = GetComponentInParent<PlayerStateManager>();
        currentState = playerStateManager.CurrentState;
    }

    public void AssignSkill(SkillSO skill)
    {
        skillSO = skill;
        skillName = skill.skillName;
        skillType = skill.skillType;
        skillAttunement = skill.skillAttunement;
        cooldown = skill.cooldown;
        icon = skill.icon;

        skillObject = Instantiate(skill.skillObject, this.transform);

        skillScript = skillObject.GetComponent<ISkill>();

        skillAOV = skill.skillAnimatorOverride;
    }

    public void ActivateSlot()
    {
        skillScript.Cast();
        animator.runtimeAnimatorController = skillAOV;
        //animator.Play("Skill", 0, 0);
        animator.CrossFadeInFixedTime("Skill", 0.25f, 0, 0.0f, 0.0f);
    }
}
