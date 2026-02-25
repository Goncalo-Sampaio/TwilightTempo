using System.Collections;
using UnityEngine;

public class MovementPlayables : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject playerHolder;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private LayerMask whatIsPlatform;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float jumpCooldown;
    [SerializeField]
    private float airMultiplier;
    [SerializeField]
    private float attackBoost = 3f;
    [SerializeField]
    private GameObject playerModel;

    private bool readyToJump = true;


    [Header("Ground Check")]
    [SerializeField]
    private float playerHeight;
    [SerializeField]
    private float groundDrag;
    [SerializeField]
    private float airDrag;
    private GroundCheck groundedComponent;
    private bool grounded;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    [Header("Jump")]
    [SerializeField]
    private AnimationClip jumpAnimation;
    [SerializeField]
    private float gravity = 9.8f;
    [SerializeField]
    private float fallMultiplier = 2f;
    [SerializeField]
    private float groundedGravity = 0.05f;
    [SerializeField]
    private AnimationCurve horizonalAirVelocityCurve;
    [SerializeField]
    private AnimationCurve verticalAirVelocityCurve;
    [SerializeField]
    private float timeToMaxXZAirVel = 1f;
    [SerializeField]
    private float timeToMaxYAirVel = 1f;
    [SerializeField]
    private float airMaxVelocity = 100f;

    private bool falling = false;
    private float airTimestamp;
    private bool wasGrounded = false;
    private bool canCheckForGround = true;

    [Header("Slope Handling")]
    [SerializeField]
    private float maxSlopeAngle;
    private RaycastHit slopeHit;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip jumpClip;
    private AudioSource audioSource;

    private PlayerStateManagerPlayables playerStateManagerPlayables;
    private PlayerStates currentPlayerState;

    //Other Scripts:
    //[SerializeField] private PlayerAnimator playerAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        groundedComponent = GetComponent<GroundCheck>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        audioSource = GetComponent<AudioSource>();
        playerStateManagerPlayables = GetComponent<PlayerStateManagerPlayables>();
        currentPlayerState = playerStateManagerPlayables.CurrentState;
    }

    private void Update()
    {
        //Debug.Log(grounded);

        if (canCheckForGround)
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
            //grounded = groundedComponent.Grounded;
        }

        if (grounded)
        {
            rb.linearDamping = groundDrag;
            falling = false;
            wasGrounded = true;
            //MAYBE PARTIALLY REDUCE VELOCITY ON THE LANDING
        }
        else
        {
            //We still need to decide if we want the air movement to be faster or slower than ground movement
            rb.linearDamping = airDrag;

            if (wasGrounded)
            {
                wasGrounded = false;
                airTimestamp = Time.time;
            }
        }

        PlayerInput();
        //Send state data to the animator
        //playerAnimator.SetPlayerMoveVars(grounded, falling, gliding, wasGrounded, canCheckForGround);
    }

    private void FixedUpdate()
    {
        currentPlayerState = playerStateManagerPlayables.CurrentState;
        playerStateManagerPlayables.SetVelocity(rb.linearVelocity.magnitude, 8f, grounded);

        if (currentPlayerState > PlayerStates.Falling)
        {
            return;
        }

        //Debug.Log(currentPlayerState);

        MovePlayer();
        SpeedControl();
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyUp(KeyCode.Space) || grounded)
        {
            //Reset falling or jumping animations back to ground animations
        }

        //if ((Input.GetKeyUp(KeyCode.Space) || jumpTimestamp + jumpDuration < Time.time) && !grounded)
        if ((Input.GetKeyUp(KeyCode.Space) || rb.linearVelocity.y < 0) && !grounded)
        {
            falling = true;
        }

        if (currentPlayerState > PlayerStates.Falling)
        {
            return;
        }

        //when to jump
        if (Input.GetKeyDown(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        float xzAirMultiplier = horizonalAirVelocityCurve.Evaluate((Time.time - airTimestamp) / timeToMaxXZAirVel);
        float yAirMultiplier = horizonalAirVelocityCurve.Evaluate((Time.time - airTimestamp) / timeToMaxYAirVel);

        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on ground
        if (grounded)
        {
            //moving on slopes
            if (OnSlope())
            {
                rb.AddForce(GetSlopeMoveDirection() * moveSpeed, ForceMode.Force);
            }
            else
            {
                rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
            }
            rb.AddForce(Vector3.down * groundedGravity, ForceMode.Acceleration);
        }
        //in air
        else if (!grounded)
        {
            //horizontal movement
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
            //rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier * xzAirMultiplier, ForceMode.Force);
            //rb.AddForce(moveDirection.normalized * moveSpeed * xzAirMultiplier, ForceMode.Force);

            //Vertical movement depending on which phase of the jump the character is in

            //falling
            if (falling)
            {
                //gravity is stronger when falling
                rb.AddForce(Vector3.down * gravity * fallMultiplier, ForceMode.Acceleration);

            }
            //jumping
            else
            {
                rb.AddForce(Vector3.down * gravity * 0.8f, ForceMode.Acceleration);
            }
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        Vector3 limitedVel;

        //airMaxVelocity = moveSpeed / 5;

        if (grounded)
        {
            //airMaxVelocity = flatVel.magnitude;

            if (Mathf.Abs(rb.linearVelocity.x) < .5f)
            //if (rb.linearVelocity.x < .5f && rb.linearVelocity.x > -.5f)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
            }

            if (rb.linearVelocity.z < .5f && rb.linearVelocity.z > -.5f)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, 0f);
            }
        }

        //limit velocity if needed
        if (flatVel.magnitude > moveSpeed && grounded)
        {
            limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
        else if (flatVel.magnitude > airMaxVelocity && !grounded)
        {
            if (airMaxVelocity < 2f)
            {
                limitedVel = flatVel.normalized;
            }
            else
            {
                limitedVel = flatVel.normalized * airMaxVelocity;
            }
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }

        if (rb.linearVelocity.magnitude < 0.5f)
        {
            //playerStateManagerPlayables.ResetState();
        }
        else
        {
            playerStateManagerPlayables.SetCurrentState(PlayerStates.Moving);
        }

        //change later when movement is refactored to use velocities instead of forces
        playerStateManagerPlayables.SetVelocity(rb.linearVelocity.magnitude, 8f, grounded);
    }

    private void Jump()
    {
        StartCoroutine(GroundedCooldownCoroutine());
        //reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        playerStateManagerPlayables.Jump(jumpAnimation);
        audioSource.PlayOneShot(jumpClip);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        airTimestamp = Time.time;
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    public void ResetVariables(bool swing = false, Vector3 direction = new Vector3())
    {
        rb.useGravity = false;

        if (swing)
        {
            airTimestamp = Time.time;

            if (direction.y > 0f)
            {
                falling = false;
            }

            rb.linearVelocity = direction;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if ((whatIsPlatform.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            gameObject.transform.parent = collision.gameObject.transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((whatIsPlatform.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            RestorePlayerHolder();
        }
    }

    private IEnumerator GroundedCooldownCoroutine()
    {
        grounded = false;
        canCheckForGround = false;
        yield return new WaitForSeconds(0.1f);
        canCheckForGround = true;
    }

    /// <summary>
    /// Checks if the player is on a slope
    /// </summary>
    /// <returns></returns>
    private bool OnSlope()
    {
        //Uses a raycast to determine if the player is on a slope
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, groundedComponent.PlayerHeight * 0.5f + 0.3f))
        {
            //Calculates the angle of the slope using the normal of the object hit
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0 && angle > 5f;
        }

        return false;
    }

    /// <summary>
    /// Calculates the move direction on slopes, based on the angle of the slope object and the player's input
    /// </summary>
    /// <returns></returns>
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public void RestorePlayerHolder()
    {
        gameObject.transform.parent = playerHolder.transform;
    }

    public void AttackBoost()
    {
        rb.AddForce(playerModel.transform.forward.normalized * moveSpeed * attackBoost, ForceMode.Force);
    }
}
