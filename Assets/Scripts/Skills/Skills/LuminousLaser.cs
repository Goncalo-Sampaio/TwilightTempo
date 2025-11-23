using UnityEngine;

public class LuminousLaser : MonoBehaviour, ISkill
{
    [SerializeField]
    private GameObject laserObject;
    [SerializeField]
    private float laserDelay = 1f;
    [SerializeField]
    private Vector3 castPosition = Vector3.zero;

    private GameObject playerModel;

    private float laserCastTimer = 0f;
    private bool castLaser = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerModel = FindAnyObjectByType<PlayerAnimEventsHandler>().gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        laserCastTimer -= Time.fixedDeltaTime;

        if (laserCastTimer <= 0)
        {
            laserCastTimer = 0;
            if (castLaser)
            {
                castLaser = false;

                transform.rotation = playerModel.transform.rotation;
                Instantiate(laserObject, transform.position + transform.TransformVector(castPosition), playerModel.transform.rotation);
            }
        }
    }

    public void Cast()
    {
        Debug.Log("Luminous Laser");
        laserCastTimer = laserDelay;
        castLaser = true;
    }
}
