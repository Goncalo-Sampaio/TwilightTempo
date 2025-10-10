using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]
    private float playerHeight;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private LayerMask whatIsPlatform;
    [SerializeField]
    private float groundCheckCubeHeight = .1f;

    public float PlayerHeight
    {
        get
        {
            return playerHeight;
        }
    }

    public bool Grounded { get; private set; }

    void OnDrawGizmos()
    {
        Vector3 groundCheckPosition = new Vector3(transform.position.x, transform.position.y - (playerHeight * 0.5f + 0.05f), transform.position.z);
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(groundCheckPosition, .5f);
        //Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - playerHeight * 0.5f + 0.2f, transform.position.z));
        Gizmos.DrawCube(groundCheckPosition, new Vector3(.45f, groundCheckCubeHeight, .45f));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 groundCheckPosition = new Vector3(transform.position.x, transform.position.y - (playerHeight * 0.5f + 0.05f), transform.position.z);
        //ground check
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        //grounded = Physics.CheckSphere(groundCheckPosition, .5f, whatIsGround);
        //bool onPlatform = Physics.CheckBox(groundCheckPosition, new Vector3(.45f, .1f, .45f), Quaternion.identity, whatIsPlatform);

        Grounded = Physics.CheckBox(groundCheckPosition, new Vector3(.45f, groundCheckCubeHeight, .45f), Quaternion.identity, whatIsGround) ||
                    Physics.CheckBox(groundCheckPosition, new Vector3(.45f, groundCheckCubeHeight, .45f), Quaternion.identity, whatIsPlatform); //|| onPlatform;
    }
}
