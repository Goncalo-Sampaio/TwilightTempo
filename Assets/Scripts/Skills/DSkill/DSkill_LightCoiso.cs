using UnityEngine;

[CreateAssetMenu(fileName = "DSkill_LightCoiso", menuName = "Game Stuff/Skill/LightCoiso")]
public class DSkill_LightCoiso : DSkill
{
    public int          totalMissiles;
    public GameObject missilePrefab;

    public class LightCoisoState : State
    {
        public LightCoisoState(State state)
        {
            cooldown = state.cooldown;
            life = state.life;
            effectObject = state.effectObject;
        }
        public int      remainingMissiles;
        public float    timer;
    }

    public override State Cast(DSkillManager player)
    {
        var baseState = base.Cast(player);

        var state = new LightCoisoState(baseState);
        state.remainingMissiles = totalMissiles;
        state.timer = 0.0f;

        return state;
    }

    public override bool Update(DSkillManager player, State state)
    {
        if (base.Update(player, state)) return false;

        var s = state as LightCoisoState;

        s.timer -= Time.time;
        if (s.timer <= 0.0f)
        {
            s.remainingMissiles--;
            if (s.remainingMissiles <= 0)
            {
                return false;
            }
            Instantiate(missilePrefab, player.transform.position, player.transform.rotation);
            s.timer = cooldown;
        }

        return true;
    }
}
