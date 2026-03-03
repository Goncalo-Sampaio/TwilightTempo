using UnityEngine;

public class CopyTransforms : MonoBehaviour
{
    [SerializeField] private bool copyPosition = true;
    [SerializeField] private bool copyRotation= true;
    [SerializeField] private bool copyScale = false;
    [SerializeField] private Transform targetObject;    
    private Quaternion originalRotation;
    private void Awake()
    {
        originalRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if(targetObject != null)
        {
            if (copyPosition) transform.position = targetObject.position;
            if (copyRotation) transform.rotation = originalRotation * targetObject.rotation;
            if (copyScale) transform.localScale = targetObject.localScale;
        }
        
    }
}
