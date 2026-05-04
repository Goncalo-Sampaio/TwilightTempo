using UnityEngine;

public class UIFollowPosition : MonoBehaviour
{
    [SerializeField] private RectTransform source;
    private RectTransform thisObject;
    private void Awake()
    {
        thisObject = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        thisObject.position = source.position;
    }
}
