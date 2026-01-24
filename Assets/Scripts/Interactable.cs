using UnityEngine;

public class Interactable : MonoBehaviour
{
    void OnInteract()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}
