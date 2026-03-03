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
        if (enemyReferences.enemyAnimator != null)
        {
            enemyReferences.enemyAnimator.Hit();
        }
        enemyReferences.enemyNavigation.StopNow(true);

        enemyReferences.flash.FlashForXIterations(1);
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
