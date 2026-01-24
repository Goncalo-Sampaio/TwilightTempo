using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class NavMeshPathRuntime : MonoBehaviour
{
    [HideInInspector]public Transform target;
    public float lineWidth = 0.1f;
    public float refreshRate = 0.1f;

    NavMeshAgent agent;
    LineRenderer line;
    NavMeshPath path;
    float timer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();

        path = new NavMeshPath();

        line.positionCount = 0;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.useWorldSpace = true;

    }

    void Update()
    {
        if (target == null)
        {
            line.positionCount = 0;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= refreshRate)
        {
            timer = 0f;
            UpdatePath();
        }
    }

    void UpdatePath()
    {
        NavMesh.CalculatePath(
        transform.position,
        target.position,
        NavMesh.AllAreas,
        path);

        Color c = path.status == NavMeshPathStatus.PathComplete
            ? Color.green
            : Color.red;

        line.startColor = c;
        line.endColor = c;

        if (path.corners.Length < 2)
        {
            line.positionCount = 0;
            return;
        }

        line.positionCount = path.corners.Length;
        line.SetPositions(path.corners);
    }
}

