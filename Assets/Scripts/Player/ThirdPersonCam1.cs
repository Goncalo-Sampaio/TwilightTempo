using UnityEngine;

public class ThirdPersonCam1 : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform playerObj;
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private float rotationSpeed;

    [Header("Cameras")]
    [SerializeField] private GameObject thirdPersonCamera;
    [SerializeField] private GameObject lockOnCamera;

    [SerializeField] private Transform lockOnTaget;
    private Vector3 midWayPoint;
    

    private CameraStyle currentStyle;

    private enum CameraStyle
    {
        Basic,
        LockOn
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // switch styles
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Basic);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCameraStyle(CameraStyle.LockOn);
        

        //Orientation defines what is forward and what is side to side.        

        // roate player object
        if(currentStyle == CameraStyle.Basic)
        {
            // rotate Orientation 
            //viewDirthe direction vector from the Orbit Camera to the player. So that the foward direciton is always where we look
            Vector3 viewDir = player.position - new Vector3(Camera.main.transform.position.x, player.position.y, Camera.main.transform.position.z);
            orientation.forward = viewDir.normalized;

            //Listens to inputs, and constructs a normalized direction vector oriented towards the Orientation GameObject
            //All this block is responsible for is checking if the inputDir is not 0 and if that's true:
            //SmoothLerp the Player's foward direction to the same direction as inputDir >>> Rotating the player model over time (Smoothly)
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            //Interpolates PlayerObject's  forward to inputDir's foward over time
            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        else if(currentStyle == CameraStyle.LockOn)
        {
            //Orientation should point to a position (inbetween the target and the player)
            //The LockOn Camera already follows the LockOnTarget object. So here you just shift the transform's position.
            //Vector3 dirToCombatLookAt = lockOnCamera.position - new Vector3(transform.position.x, lockOnCamera.position.y, transform.position.z);
            //orientation.forward = dirToCombatLookAt.normalized;

            //playerObj.forward = dirToCombatLookAt.normalized;

            Vector3 viewDir = lockOnTaget.position - new Vector3(playerObj.transform.position.x, lockOnTaget.position.y, playerObj.transform.position.z);
            orientation.forward = viewDir.normalized;
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            //Interpolates PlayerObject's  forward to inputDir's foward over time
            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {       
        thirdPersonCamera.SetActive(false);
        lockOnCamera.SetActive(false);

        if (newStyle == CameraStyle.Basic) thirdPersonCamera.SetActive(true);
        if (newStyle == CameraStyle.LockOn) lockOnCamera.SetActive(true);

        currentStyle = newStyle;
    }
}
