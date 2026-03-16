using UnityEngine;

public class EnemyState_Combat : IState
{
    private EnemyReferences enemyReferences;

    private Transform playerRef;
    private float storedUpdateFrequency;
    private float attackUpdateFrequency; // via a scriptable object
    private float attackTimer;
    private bool isCaster;

    private StateMachine combatSubStateMachine;
    public EnemyState_Combat(EnemyReferences enemyReferences, float attackUpdateFrequency)
    {
        this.isCaster = enemyReferences.isCaster;
        this.enemyReferences = enemyReferences;
        playerRef = enemyReferences.playerRef.transform;
        this.attackUpdateFrequency = attackUpdateFrequency;
        storedUpdateFrequency = attackUpdateFrequency;

        //create the combat substate machine
    }
    public void OnEnter()
    {
        if (enemyReferences.enemyBrain.isBerserk) BerserkMode();
        Debug.Log("Combat");
        //The first attack happens emediatly
        attackTimer = attackUpdateFrequency/4f;

    }
    public void Tick()
    {
        enemyReferences.enemyNavigation.LookAtTarget(playerRef.position);
        if (AttackUpdate())
        {
            if(isCaster)
            {
                if (enemyReferences.enemyNavigation.GetVisionConeFactor(playerRef.position) < 0.5f) enemyReferences.enemyNavigation.LookAtTarget(playerRef.position);
                else if (enemyReferences.enemyAnimator != null)
                {
                    enemyReferences.enemyNavigation.SnapToTarget(playerRef.position);
                    Debug.Log("enemyReferences.enemyNavigation.SnapToTarget(playerRef.position)");
                    enemyReferences.enemyAnimator.SpellCast();

                    Debug.Log("enemyReferences.enemyAnimator.SpellCast();");
                    enemyReferences.enemyCasterAttack.CastSpell();
                    Debug.Log(" enemyReferences.enemyCasterAttack.CastSpell();");
                }
            }
            else
            {

                enemyReferences.enemeyAttack.Attacking();
                if (enemyReferences.enemyAnimator != null)
                {
                    enemyReferences.enemyNavigation.SnapToTarget(playerRef.position);
                    enemyReferences.enemyAnimator.Attack1();
                }

            }
        }
    }
    //Make sure the state values Reset when leaving.
    public void OnExit()
    {
        attackTimer = attackUpdateFrequency;
        //enable navigation:
        enemyReferences.enemyNavigation.StopNow(false);
        enemyReferences.enemeyAttack.StopAttacking();

    }
    private void BerserkMode() => attackUpdateFrequency = storedUpdateFrequency / 2f;
    private bool AttackUpdate()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            attackTimer = attackUpdateFrequency;
            return true;
        }
        else return false;
    }
    public Color GizmoColor()
    {
        return Color.orange;
    }
    



}
