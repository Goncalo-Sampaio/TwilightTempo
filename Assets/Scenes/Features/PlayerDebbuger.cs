using UnityEditor;
using UnityEngine;

public class PlayerDebbuger : MonoBehaviour
{
    [SerializeField] private bool displayText = false;

    [Header("Orientation")]
    [SerializeField] private bool showOrientationDirection = true;
    [SerializeField] private GameObject orientation;
    [SerializeField] private float orientLineLenght = 2f;
    [SerializeField] private float orientLineHeightOffset = 1f;

    [Header("Player")]
    [SerializeField] private bool showPlayerDirection = true;
    [SerializeField] private GameObject player;
    [SerializeField] private float playerLineLenght = 2f;
    [SerializeField] private float playerLineHeightOffset = 1f;

    private void OnDrawGizmos()
    {
        
        if (showOrientationDirection)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(orientation.transform.position
            + Vector3.up * orientLineHeightOffset,
            orientation.transform.position
            + orientation.transform.forward * orientLineLenght
            + Vector3.up * orientLineHeightOffset);

        }
        if (showPlayerDirection)
        {
            Gizmos.color = Color.hotPink;
            Gizmos.DrawLine(player.transform.position
            + Vector3.up * playerLineHeightOffset,
            player.transform.position
            + player.transform.forward * playerLineLenght
            + Vector3.up * playerLineHeightOffset);

        }

        if (!displayText) return;
        else
        {
            Handles.Label(orientation.transform.position
                + orientation.transform.forward * orientLineLenght / 2
                + Vector3.up * orientLineHeightOffset
                + Vector3.up * .2f
                , $"ORIENTATION");
            Handles.Label(player.transform.position
                + player.transform.forward * playerLineLenght / 2
                + Vector3.up * playerLineHeightOffset
                + Vector3.up * .2f
                , $"PLAYER");


        }

    }
}
