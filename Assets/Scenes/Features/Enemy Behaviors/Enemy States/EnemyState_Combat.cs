using UnityEngine;

public class EnemyState_Combat : IState
{
    private EnemyReferences enemyReferences;

    private Transform playerRef;

    private float attackUpdateFrequency; // via a scriptable object
    private float attackTimer;

    private StateMachine combatSubStateMachine;
    public EnemyState_Combat(EnemyReferences enemyReferences, Transform player, float attackUpdateFrequency)
    {
        this.enemyReferences = enemyReferences;
        playerRef = player;
        this.attackUpdateFrequency = attackUpdateFrequency;        

        //create the combat substate machine
    }
    public void OnEnter()
    {
        Debug.Log("Combat");
        //The first attack happens almost emediatly
        attackTimer = attackUpdateFrequency / 4f;
        //disable navigation
        enemyReferences.enemyNavigation.StopNow(true);

    }
    public void Tick()
    {
        enemyReferences.enemyNavigation.LookAtTarget(playerRef.position);
        if (AttackUpdate())
        {
            enemyReferences.enemeyAttack.AttackPlayer();
            if (enemyReferences.enemyAnimator != null)
            {
                enemyReferences.enemyNavigation.SnapToTarget(playerRef.position);
                enemyReferences.enemyAnimator.Attack1();
            }
        }
        Debug.Log(enemyReferences.enemyNavigation.LinearDistanceFromTarget(playerRef.position));
    }
    //Make sure the state values Reset when leaving.
    public void OnExit()
    {
        attackTimer = attackUpdateFrequency;
        //enable navigation:
        enemyReferences.enemyNavigation.StopNow(false);

    }
    public Color GizmoColor()
    {
        return Color.orange;
    }
    
    public bool OutsideAttackRange(float attackRange) => enemyReferences.enemyNavigation.LinearDistanceFromTarget(playerRef.position) > attackRange;
    public bool HasLineOfSight (Vector3 target) => enemyReferences.enemyNavigation.HasLineOfSight(target);


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



}
