using NaughtyAttributes;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCasterAttack : MonoBehaviour
{
    [SerializeField] private Animator attackAnimator;
    private bool attacking = false;
    [SerializeField] private float castProjectileAt = 0.45f;
    private int attack01AnimHash = Animator.StringToHash("Base Layer.Spell");

    [SerializeField] private Transform projectileExit;
    [SerializeField] private GameObject SpellProjectile;
    
    [Button]
    public void CastSpell() { if (!attacking) StartCoroutine(CastSpellRot()); }
    private IEnumerator CastSpellRot()
    {
        attacking = true;
        Debug.Log("CastSpellRotStarted");
        //enable tatto and eye glows
        //enable staff glow
        //enable staff magic particles

        yield return new WaitUntil(() => (CurrentAnimationCompletion() >= castProjectileAt) && CurrentAnimationCompletion() < 1);
       
        Instantiate(SpellProjectile, projectileExit.position, projectileExit.rotation);
        
        Debug.Log(" Instantiated Spell");
        yield return null;
        //lerp tatto and eyes glows back to 0
        //same for staff glow
        attacking = false;
        Debug.Log("CastSpellRotStarted Done");
    }
    private float CurrentAnimationCompletion()
    {
        AnimatorStateInfo stateInfo = attackAnimator.GetCurrentAnimatorStateInfo(0);        
        if (attack01AnimHash == stateInfo.fullPathHash)
        {
            //Debug.Log("Spell anim clip at: " + stateInfo.normalizedTime);
            return stateInfo.normalizedTime;
        }
        else return -1;
    }
}
