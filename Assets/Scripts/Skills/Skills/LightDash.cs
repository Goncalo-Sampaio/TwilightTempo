using UnityEngine;

public class LightDash : MonoBehaviour, ISkill
{
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private float duration;

    private PlayerMov2 playerMovement;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMov2>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Cast()
    {
        Debug.Log("Light Dash");
    }
}
