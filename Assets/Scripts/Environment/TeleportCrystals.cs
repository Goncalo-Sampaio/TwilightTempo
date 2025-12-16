using UnityEngine;

public class TeleportCrystals : MonoBehaviour
{
    [SerializeField]
    private GameObject targetCrystal;
    [SerializeField]
    private float interactionRange = 5f;

    private Teleport player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Teleport>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && Vector3.Distance(player.transform.position, gameObject.transform.position) < interactionRange)
        {
            player.ActivateTeleport(targetCrystal);
        }
    }
}
