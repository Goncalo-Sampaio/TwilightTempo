using UnityEngine;
using UnityEngine.AI;

public class NavigationTesting : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }
    public void LookAtTarget(Vector3 target) => transform.LookAt(target);
}
