using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WaypointHandler : MonoBehaviour
{
    public List<Transform> wayPoints;
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
}

