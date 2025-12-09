using UnityEngine;

public class PlayerCinematics : MonoBehaviour
{
    [SerializeField]
    private GameObject finisherCinematic;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateFinisher()
    {
        finisherCinematic.SetActive(true);
    }

    public void EndFinisher()
    {
        finisherCinematic.SetActive(false);
    }
}
