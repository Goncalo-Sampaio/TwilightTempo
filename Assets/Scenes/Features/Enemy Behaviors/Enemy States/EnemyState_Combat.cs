using UnityEngine;

public class EnemyState_Combat : IState
{
    private EnemyReferences enemyReferences;
    private StateMachine combatSubStateMachine;
    public EnemyState_Combat(EnemyReferences enemyReferences)
    {
        this.enemyReferences = enemyReferences;

        //create the combat substate machine
    }
    public void OnEnter()
    {
        Debug.Log("Combat");
    }
    //Make sure the state values Reset when leaving.
    public void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public void Tick()
    {
        throw new System.NotImplementedException();
    }
    public Color GizmoColor()
    {
        return Color.orange;
    }



}
