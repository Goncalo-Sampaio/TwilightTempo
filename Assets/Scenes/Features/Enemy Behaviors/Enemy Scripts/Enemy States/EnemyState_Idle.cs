using UnityEngine;

public class EnemyState_Idle : IState
{    
    private EnemyReferences enemyReferences;
    private float timeTillPatroll;
    private float currentTime;
    private bool triggerPatroll = false;
    public EnemyState_Idle(EnemyReferences enemyReferences, float timeTillPatroll)
    {
        this.enemyReferences = enemyReferences;
        this.timeTillPatroll = timeTillPatroll;
    }
    public void OnEnter()
    {
        enemyReferences.enemyBrain.isPatrolling = false;
        triggerPatroll = false;
        currentTime = Time.time;
        //if (enemyReferences.enemyAnimator != null)
        //{
        //    if (enemyReferences.enemyBrain.isPatrolling) return;
        //    enemyReferences.enemyAnimator.StartIdle();
        //}
        Debug.Log("Idle OnEnter");
    }
    public void Tick()
    {
        //add random
        if (Time.time < timeTillPatroll + Random.Range(-1.0f,1.0f) + currentTime) return;
        if (!triggerPatroll)
        {
            enemyReferences.enemyBrain.StartPatrol();
            triggerPatroll = true;
        }
    }
    //Make sure the state values Reset when leaving.
    public void OnExit()
    {
        Debug.Log("Idle OnExit");
        //if (enemyReferences.enemyAnimator != null)
        //{
        //    if (enemyReferences.enemyBrain.isPatrolling) return;
        //    enemyReferences.enemyAnimator.StopIdle();
        //}
        enemyReferences.enemyBrain.StopPatrol();
        triggerPatroll = false;
        enemyReferences.enemyBrain.isPatrolling = false;
    }

    

    public Color GizmoColor()
    {
        return Color.blue;
    }
    

    
}
