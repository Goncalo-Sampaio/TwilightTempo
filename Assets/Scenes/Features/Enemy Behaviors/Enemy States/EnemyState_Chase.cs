using UnityEngine;

public class EnemyState_Chase : IState
{
    private EnemyReferences enemyReferences;
    public EnemyState_Chase(EnemyReferences enemyReferences)
    {
        this.enemyReferences = enemyReferences;
    }
    public void OnEnter()
    {
        Debug.Log("Chase OnEnter");
    }

    public void OnExit()
    {
        Debug.Log("Chase OnExit");
    }

    public void Tick()
    {
        Debug.Log("Chase Tick");
    }
    public Color GizmoColor()
    {
        return Color.purple;
    }
    //go trough Navigation Class for this. Not directly
    public bool HasArrivedAtTarget()
    {
        return enemyReferences.navMeshAgent.remainingDistance < 0.1f; //switch this out with editable variable
    }

}
