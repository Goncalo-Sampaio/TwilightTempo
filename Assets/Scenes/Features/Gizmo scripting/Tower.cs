using UnityEditor;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private float radius = 1f;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Handles.Label(transform.position + Vector3.up * (radius +.2f), $"radius = {radius}");
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
