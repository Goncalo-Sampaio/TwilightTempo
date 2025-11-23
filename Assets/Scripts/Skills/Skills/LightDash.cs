using UnityEngine;

public class LightDash : MonoBehaviour, ISkill
{
    [SerializeField]
    private GameObject lightDashObject;
    [SerializeField]
    private Vector3 castPosition = Vector3.zero;
    [SerializeField]
    private float dashDelay = 0.57f;

    private GameObject playerModel;

    private float dashCastTimer = 0f;
    private bool castDash = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerModel = FindAnyObjectByType<PlayerAnimEventsHandler>().gameObject;
    }

    void FixedUpdate()
    {
        dashCastTimer -= Time.fixedDeltaTime;

        if (dashCastTimer <= 0)
        {
            dashCastTimer = 0;
            if (castDash)
            {
                castDash = false;

                transform.rotation = playerModel.transform.rotation;
                Instantiate(lightDashObject, transform.position + transform.TransformVector(castPosition), playerModel.transform.rotation, transform);
            }
        }
    }

    public void Cast()
    {
        Debug.Log("Light Dash");
        dashCastTimer = dashDelay;
        castDash = true;
    }
}
