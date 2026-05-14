using UnityEngine;

[CreateAssetMenu(fileName = "PlayerReferences", menuName = "Scriptable Objects/PlayerReferences")]
public class PlayerReferences : ScriptableObject
{
    [SerializeField] private GameObject skillTimer1;
    [SerializeField] private GameObject skillTimer2;
    [SerializeField] private GameObject skillTimer3;
    [SerializeField] private GameObject skillTimer4;
    [SerializeField] private GameObject skillTimer5;
}
