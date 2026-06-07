using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WaypointHandler : MonoBehaviour
{
    public List<Transform> wayPoints = new();
   
    public Transform GetClosestWaypoint(Vector3 origin)
    {
        Transform closestWayPoint = null;

        float closestDistance = 1000f;
        foreach (Transform t in wayPoints)
        {
            float distance = (origin - t.position).magnitude;
            if (distance < closestDistance)
            {
                closestWayPoint = t;
                closestDistance = distance;
            }
        }
        return closestWayPoint;
    }
    private void OnDrawGizmos()
    {
        if (wayPoints != null)
        {
            Gizmos.color = Color.pink;
            for (int i = 0; i < wayPoints.Count; i++)
            {
                if (i == wayPoints.Count -1)
                {
                    Gizmos.DrawLine(wayPoints[wayPoints.Count - 1].position, wayPoints[0].position);
                }
                else Gizmos.DrawLine(wayPoints[i].position, wayPoints[i + 1].position);
            }
            Gizmos.color = Color.yellow;
            for (int i = 0; i < wayPoints.Count; i++) Gizmos.DrawWireSphere(wayPoints[i].position, .2f);
        }

    }
}

