using System.Threading;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    [SerializeField]
    private Transform skillHolder;
    [SerializeField]
    private float rotationTime = 1f; 

    private Vector3 rightRotation = new Vector3(0, 0, -60);
    private Vector3 leftRotation = new Vector3(0, 0, 60);
    private bool rotating = false;
    private float rotationProgress = 0;
    private Vector3 initialRotation;
    private Vector3 finalRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !rotating)
        {
            rotating = true;
            rotationProgress = 0;
            //skillHolder.Rotate(rightRotation);
            initialRotation = skillHolder.rotation.eulerAngles;
            finalRotation = skillHolder.rotation.eulerAngles + rightRotation;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && !rotating)
        {
            rotating = true;
            rotationProgress = 0;
            //skillHolder.Rotate(leftRotation);
            initialRotation = skillHolder.rotation.eulerAngles;
            finalRotation = skillHolder.rotation.eulerAngles + leftRotation;
        }

        if (rotating)
        {
            rotationProgress = rotationProgress + Time.deltaTime / rotationTime;
            skillHolder.rotation = Quaternion.Lerp(Quaternion.Euler(initialRotation), Quaternion.Euler(finalRotation), rotationProgress);


            if (rotationProgress >= 1)
            {
                rotating = false;
            }
        }
    }
}
