using UnityEngine;

[CreateAssetMenu(menuName ="Attacks/Normal Attack")]
public class AttacksSO : ScriptableObject
{
    [SerializeField]
    private AnimatorOverrideController animatorOV;
    [SerializeField]
    private float damage;

    public AnimatorOverrideController AnimatorOV
    {
        get { return animatorOV; }
    }

    public float Damage
    {
        get { return damage; }
    }
}
