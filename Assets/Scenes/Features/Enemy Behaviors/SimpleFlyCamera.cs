using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleFlyCamera : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;
    public float sprintMultiplier = 3f;

    public float interactDistance = 100f;

    float yaw;
    float pitch;
    bool cameraActive = true;

    
    private void Awake()
    {
        
        LockCursor(true);
    }
    void Start()
    {
        
        
    }
    
    

    void Update()
    {
        // Toggle camera mode
        if (Input.GetKeyDown(KeyCode.O))
        {
            cameraActive = !cameraActive;
            LockCursor(cameraActive);
        }

        if (cameraActive)
        {
            FlyCameraUpdate();
        }
        else
        {
            InteractionUpdate();
        }
    }

    void FlyCameraUpdate()
    {
        // Mouse look
        yaw += Input.GetAxis("Mouse X") * lookSpeed;
        pitch -= Input.GetAxis("Mouse Y") * lookSpeed;
        pitch = Mathf.Clamp(pitch, -89f, 89f);
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        // Movement
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= sprintMultiplier;

        Vector3 direction = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );

        if (Input.GetKey(KeyCode.E))
            direction.y += 1;
        if (Input.GetKey(KeyCode.Q))
            direction.y -= 1;

        transform.Translate(direction * speed * Time.deltaTime, Space.Self);
    }

    void InteractionUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if(hit.transform.GetComponent<MeshRenderer>() != null)
                {

                    


                    Debug.Log("Hit: " + hit.collider.name);

                   
                    hit.collider.SendMessage(
                        "OnInteract",
                        SendMessageOptions.DontRequireReceiver
                    );

                }
                
            }
        }
    }

    void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    
}
