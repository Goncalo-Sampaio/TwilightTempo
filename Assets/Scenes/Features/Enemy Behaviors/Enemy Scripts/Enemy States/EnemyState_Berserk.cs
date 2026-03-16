using UnityEngine;

public class EnemyState_Berserk : IState
{
    private EnemyReferences enemyReferences;
    public EnemyState_Berserk(EnemyReferences enemyReferences)
    {
        this.enemyReferences = enemyReferences;
    }
    public void OnEnter()
    {
        enemyReferences.enemyNavigation.StopNow(true);
    }

    public void OnExit()
    {
        enemyReferences.enemyNavigation.StopNow(false);
    }

    public void Tick()
    {
        
    }
    public Color GizmoColor()
    {
        return Color.white;
    }
}
