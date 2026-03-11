using UnityEngine;

public class EnemyState_Chase : IState
{
    private EnemyReferences enemyReferences;
    private EnemyNavigation enemyNav;

    private Transform playerRef;

    private float chaseUpdateFrequency; // via a scriptable object
    private float chaseTimer;
    public EnemyState_Chase(EnemyReferences enemyReferences,float chaseUpdateFrequency)
    {
        this.enemyReferences = enemyReferences;
        enemyNav = enemyReferences.enemyNavigation;
        playerRef = enemyReferences.playerRef.transform;        
        this.chaseUpdateFrequency = chaseUpdateFrequency;
    }
    public void OnEnter()
    {
        enemyReferences.enemyNavigation.StopNow(false);
        if (enemyReferences.enemyAnimator != null)
        {
            enemyReferences.enemyAnimator.StartRunning();
        }        
        chaseTimer = 0f;
    }
    public void Tick()
    {
        if (ChaseUpdate()) enemyNav.MoveTo(playerRef.position);
    }

    public void OnExit()
    {
        if (enemyReferences.enemyAnimator != null)
        {
            enemyReferences.enemyAnimator.StopRunning();
        }
        enemyReferences.enemyNavigation.StopNow(true);
        chaseTimer = chaseUpdateFrequency;
    }
    private bool ChaseUpdate()
    {
        chaseTimer -= Time.deltaTime;
        if (chaseTimer <= 0f)
        {
            chaseTimer = chaseUpdateFrequency;
            return true;
        }
        else return false;
    }
    


    public Color GizmoColor()
    {
        return Color.purple;
    }
    
    

}
