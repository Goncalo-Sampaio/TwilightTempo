using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WayPoints : MonoBehaviour
{
    [SerializeField] private bool isLooping = false;
    private void OnDrawGizmos()
    {
        for (int i = 0;i < transform.childCount-1;i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
        Gizmos.color = Color.white;
        if (isLooping)
        {
            Gizmos.DrawLine(transform.GetChild(transform.childCount-1).position, transform.GetChild(0).position);
        }
    }
}
