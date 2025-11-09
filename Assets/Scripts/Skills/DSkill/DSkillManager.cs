using UnityEngine;

public class DSkillManager : MonoBehaviour
{
    public DSkill[] skills;

    DSkill.State[] skillState;

    void Start()
    {
        skillState = new DSkill.State[skills.Length];
        for (int i = 0; i < skills.Length; i++) skillState[i] = null;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skillState[i] == null)
            {
                if (Input.GetKeyDown((KeyCode)(KeyCode.Alpha1 + i)))
                {
                    skillState[i] = skills[i].Cast(this);
                }
            }
            else
            {
                if (!skills[i].Update(this, skillState[i]))
                {
                    skillState[i] = null;
                }
            }
        }
    }
}
