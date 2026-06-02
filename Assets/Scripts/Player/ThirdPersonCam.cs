using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform playerObj;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private Transform combatLookAt;

    [SerializeField]
    private GameObject thirdPersonCam;
    [SerializeField]
    private GameObject combatCam;

    private bool attacking = false;

    public bool Attacking
    {
        get
        {
            return attacking;
        } 
        set
        {
            attacking = value;
        }
    }

    [SerializeField]
    private float skillRotationTime = 0.1f;
    [SerializeField]
    private bool skill = false;
    private float skillRotationCounter = 0f;

    private CameraStyle currentStyle;

    private PlayerStateManagerPlayables playerStateManager;
    private PlayerStates currentState;

    private enum CameraStyle
    {
        Basic,
        Combat
    }

    private void Start()
    {
        playerStateManager = GetComponentInParent<PlayerStateManagerPlayables>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        currentState = playerStateManager.CurrentState;

        // switch styles
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Basic);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCameraStyle(CameraStyle.Combat);

        // rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // roate player object
        if(currentStyle == CameraStyle.Basic)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir.magnitude >= 0.2f && currentState <= PlayerStates.Skill)
            //if (inputDir != Vector3.zero && currentState <= PlayerStates.Skill)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
            else if (skill)
            {
                playerObj.forward = orientation.forward.normalized;
            }
            else if (attacking)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, orientation.forward.normalized, Time.deltaTime * rotationSpeed);
            }
        }

        if (skill)
        {
            skillRotationCounter -= Time.deltaTime;

            if (skillRotationCounter <= 0)
            {
                skill = false;
            }
        }

        else if(currentStyle == CameraStyle.Combat)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerObj.forward = dirToCombatLookAt.normalized;
        }
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);

        currentStyle = newStyle;
    }

    public void StartSkill()
    {
        skillRotationCounter = skillRotationTime + 0.1f;
        skill = true;
    }
}
