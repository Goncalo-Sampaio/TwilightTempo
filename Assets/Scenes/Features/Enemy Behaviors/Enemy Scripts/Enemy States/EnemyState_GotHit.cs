using UnityEngine;

public class EnemyState_GotHit : IState
{
    //Implement stutter and flash here
    private EnemyReferences enemyReferences;
    
    public EnemyState_GotHit (EnemyReferences enemyReferences)
    {
        this.enemyReferences = enemyReferences;        
    }
    public void OnEnter()
    {
        enemyReferences.enemyBrain.isPatrolling = false;
        enemyReferences.enemyNavigation.StopNow(true);

        
        enemyReferences.enemyBrain.isPatrolling = false;
        Debug.Log("EnemyState_GotHit OnEnter()");
        
    }
    public void Tick()
    {
        Debug.Log("EnemyState_GotHit.Tick()");
    }

    public void OnExit()
    {
        Debug.Log("EnemyState_GotHit.OnExit()");
    }
    
    public Color GizmoColor()
    {
        return Color.indianRed;
    }
    

}
