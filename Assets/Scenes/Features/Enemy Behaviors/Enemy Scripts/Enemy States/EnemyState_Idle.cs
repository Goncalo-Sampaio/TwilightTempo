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
            enemyReferences.enemyAnimator.StartIdle();
        }
        Debug.Log("Idle OnEnter");
    }
    //Make sure the state values Reset when leaving.
    public void OnExit()
    {
        Debug.Log("Idle OnExit");
        if (enemyReferences.enemyAnimator != null)
        {
            enemyReferences.enemyAnimator.StopIdle();
        }
    }

    public void Tick()
    {
        
    }

    public Color GizmoColor()
    {
        return Color.blue;
    }
    

    
}
