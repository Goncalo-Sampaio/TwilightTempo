using UnityEngine;

public class EnemyState_Chase : IState
{
    private EnemyReferences enemyReferences;
    private EnemyNavigation enemyNav;

    private Transform playerRef;

    private float chaseUpdateFrequency; // via a scriptable object
    private float chaseTimer;
    public EnemyState_Chase(EnemyReferences enemyReferences,Transform player,float chaseUpdateFrequency)
    {
        this.enemyReferences = enemyReferences;
        enemyNav = enemyReferences.enemyNavigation;
        playerRef = player;        
        this.chaseUpdateFrequency = chaseUpdateFrequency;
    }
    public void OnEnter()
    {
        Debug.Log("Chase OnEnter");
        chaseTimer = chaseUpdateFrequency;
    }
    public void Tick()
    {
        Debug.Log($"Chase Tick. Remaining distance: {enemyNav.NavMeshDistanceFromTarget()}");
        
        if (ChaseUpdate()) enemyNav.MoveTo(playerRef.position);
    }

    public void OnExit()
    {
        Debug.Log("Chase OnExit");
        chaseTimer = chaseUpdateFrequency;
    }

    
    public Color GizmoColor()
    {
        return Color.purple;
    }
    public bool Arrived(float minDistance) => enemyReferences.enemyNavigation.HasArrivedAtTarget(minDistance);
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

}
