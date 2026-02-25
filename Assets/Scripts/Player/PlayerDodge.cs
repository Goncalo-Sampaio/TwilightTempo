using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField]
    private float velocity;
    [SerializeField]
    private float dodgeSpeedMultiplier = 1;
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private AnimationClip dodgeAnimation;

    private PlayerStateManagerPlayables playerStateManagerPlayables;

    private bool canDodge = true;
    private bool dodging = false;
    private float dodgeTimer = 0f;
    private bool resetState = false;

    private Rigidbody rb;
    private Transform playerModel;

    private void Start()
    {
        playerStateManagerPlayables = GetComponent<PlayerStateManagerPlayables>();
        rb = GetComponent<Rigidbody>();
        playerModel = GetComponentInChildren<PlayerAnimEventsHandler>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDodge)
        {
            dodgeTimer = dodgeAnimation.length / dodgeSpeedMultiplier;
            canDodge = false;
            dodging = true;
            resetState = true;
            playerStateManagerPlayables.SetCurrentState(PlayerStates.Dashing);
            playerStateManagerPlayables.Attack(dodgeAnimation, dodgeSpeedMultiplier);
        }
    }

    private void FixedUpdate()
    {
        if (dodging)
        {
            rb.linearVelocity = playerModel.transform.forward * velocity * Time.fixedDeltaTime;
        }

        dodgeTimer -= Time.fixedDeltaTime;

        if (dodgeTimer < 0f)
        {
            dodgeTimer = 0f;
            dodging = false;
            canDodge = true;

            if (resetState)
            {
                resetState = false;
                playerStateManagerPlayables.ResetState();
            }
        }
    }
}
