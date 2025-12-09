using UnityEngine;
using UnityEngine.AI;

public class Area : MonoBehaviour
{
    public float radius = 10f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public Vector3 GetRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection.y = 0f;

        Vector3 randomPoint = transform.position + randomDirection;

        NavMeshHit hit;
        Vector3 finalPosition = transform.position;
        
        //Queries NavMesh for valid position around randomPoint + radius.
        if (NavMesh.SamplePosition(randomPoint,out hit,2f,1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
