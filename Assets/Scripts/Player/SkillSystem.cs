using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    [SerializeField]
    private Transform skillHolder;
    [SerializeField]
    private float rotationTime = 1f;
    [SerializeField]
    private List<SkillSlot> skillSlots = new List<SkillSlot>();
    [SerializeField]
    private List<SkillSO> skillSOs = new List<SkillSO>();

    private Vector3 rightRotation = new Vector3(0, 0, -60);
    private Vector3 leftRotation = new Vector3(0, 0, 60);
    private bool rotating = false;
    private float rotationProgress = 0;
    private Vector3 initialRotation;
    private Vector3 finalRotation;

    private int currentlyActiveSlot = 0;

    private PlayerStateManagerPlayables playerStateManager;
    private PlayerStates state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateManager = GetComponent<PlayerStateManagerPlayables>();

        currentlyActiveSlot = 0;

        for (int i = 0; i < skillSlots.Count; i++)
        {
            skillSlots[i].AssignSkill(skillSOs[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        state = playerStateManager.CurrentState;

        if (Input.GetKeyDown(KeyCode.E) && !rotating)
        {
            rotating = true;
            rotationProgress = 0;
            //skillHolder.Rotate(rightRotation);
            initialRotation = skillHolder.rotation.eulerAngles;
            finalRotation = skillHolder.rotation.eulerAngles + rightRotation;

            currentlyActiveSlot--;
            if (currentlyActiveSlot < 0)
            {
                currentlyActiveSlot = skillSlots.Count - 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q) && !rotating)
        {
            rotating = true;
            rotationProgress = 0;
            //skillHolder.Rotate(leftRotation);
            initialRotation = skillHolder.rotation.eulerAngles;
            finalRotation = skillHolder.rotation.eulerAngles + leftRotation;

            currentlyActiveSlot++;
            if (currentlyActiveSlot >= skillSlots.Count)
            {
                currentlyActiveSlot = 0;
            }
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

        if (Input.GetButtonDown("Fire2") && state < PlayerStates.Skill)
        {
            skillSlots[currentlyActiveSlot].ActivateSlot();
        }
    }
}
