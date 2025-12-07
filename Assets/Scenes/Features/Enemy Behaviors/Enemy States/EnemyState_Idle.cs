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
        Debug.Log("Idle OnEnter");
    }
    //Make sure the state values Reset when leaving.
    public void OnExit()
    {
        Debug.Log("Idle OnExit");
    }

    public void Tick()
    {
        Debug.Log("Idle Tick");
    }
    public Color GizmoColor()
    {
        return Color.blue;
    }
    

    
}
