using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public SkillType skillType;
    public SkillAttunement skillAttunement;
    public float cooldown;
    public Sprite icon;
    public GameObject skillObject;
    public AnimatorOverrideController skillAnimatorOverride;
}
