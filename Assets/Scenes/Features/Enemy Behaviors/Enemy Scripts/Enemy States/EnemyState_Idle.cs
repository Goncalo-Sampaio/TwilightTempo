using UnityEngine;

public class EnemyState_Idle : IState
{    
    private EnemyReferences enemyReferences;
    
    public EnemyState_Idle(EnemyReferences enemyReferences)
    {
        this.enemyReferences = enemyReferences;
    }
    public void OnEnter()
    {
        if (enemyReferences.enemyAnimator != null)
        {
            if (enemyReferences.enemyBrain.isPatrolling) return;
            enemyReferences.enemyAnimator.StartIdle();
        }
        Debug.Log("Idle OnEnter");
    }
    public void Tick()
    {
        
    }
    //Make sure the state values Reset when leaving.
    public void OnExit()
    {
        Debug.Log("Idle OnExit");
        if (enemyReferences.enemyAnimator != null)
        {
            if (enemyReferences.enemyBrain.isPatrolling) return;
            enemyReferences.enemyAnimator.StopIdle();
        }
    }

    

    public Color GizmoColor()
    {
        return Color.blue;
    }
    

    
}
