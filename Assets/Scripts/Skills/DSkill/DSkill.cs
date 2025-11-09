using UnityEngine;

public class DSkill : ScriptableObject
{
    public class State
    {
        public float        cooldown;
        public float        life;
        public GameObject   effectObject;
    }

    public float cooldown;
    public float duration;
    public GameObject prefab;

    public virtual State Cast(DSkillManager player)
    {
        GameObject effectObject = null;
        if (prefab)
        {
            effectObject = Instantiate(prefab, player.transform.position, player.transform.rotation);
        }

        return new State
        {
            cooldown = cooldown,
            life = duration,
            effectObject = effectObject
        };
    }

    public virtual bool Update(DSkillManager player, State state)
    {
        state.life -= Time.deltaTime;

        if (state.life <= 0) return false;

        return true;
    }
}
