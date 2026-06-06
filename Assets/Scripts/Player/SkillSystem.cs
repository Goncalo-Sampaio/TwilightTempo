using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Audio;

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
    [SerializeField]
    private AudioClip skillSwitchClip;

    private Vector3 rightRotation = new Vector3(0, 0, -60);
    private Vector3 leftRotation = new Vector3(0, 0, 60);
    private bool rotating = false;
    private float rotationProgress = 0;
    private Vector3 initialRotation;
    private Vector3 finalRotation;

    private int currentlyActiveSlot = 0;

    private PlayerStateManagerPlayables playerStateManager;
    private PlayerStates state;
    private ThirdPersonCam thirdPersonCam;
    private AudioSource audioSource;

    public bool Paused { get; set; } = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateManager = GetComponent<PlayerStateManagerPlayables>();
        audioSource = GetComponent<AudioSource>();
        thirdPersonCam = GetComponentInChildren<ThirdPersonCam>();

        currentlyActiveSlot = 0;

        for (int i = 0; i < skillSlots.Count; i++)
        {
            skillSlots[i].AssignSkill(skillSOs[i]);
        }        
    }
    private void OnEnable()
    {
        LevelDataManager.onCanvasRegister += GetSkillHolder;
    }
    private void OnDisable()
    {
        LevelDataManager.onCanvasRegister -= GetSkillHolder;
    }
    private void GetSkillHolder() => skillHolder = LevelDataManager.Instance.playerCanvas.skillHolder.transform;

    // Update is called once per frame
    void Update()
    {
        if (Paused)
        {
            return;
        }

        state = playerStateManager.CurrentState;

        if (Input.GetKeyDown(KeyCode.E) && !rotating)
        {
            if (!CheckForSkillHolderReference()) return;
            audioSource.PlayOneShot(skillSwitchClip);
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
            if (!CheckForSkillHolderReference()) return;
            audioSource.PlayOneShot(skillSwitchClip);
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
            if (!CheckForSkillHolderReference()) return;
            rotationProgress = rotationProgress + Time.deltaTime / rotationTime;
            skillHolder.rotation = Quaternion.Lerp(Quaternion.Euler(initialRotation), Quaternion.Euler(finalRotation), rotationProgress);

            if (rotationProgress >= 1)
            {
                rotating = false;
            }
        }

        if (Input.GetButtonDown("Fire2") && state < PlayerStates.Skill)
        {
            thirdPersonCam.StartSkill();
            //HARDCODED for now

            if (currentlyActiveSlot + 3 >= skillSlots.Count)
            {
                skillSlots[currentlyActiveSlot - 3].ActivateCooldown();
            }
            else
            {
                skillSlots[currentlyActiveSlot + 3].ActivateCooldown();
            }

            skillSlots[currentlyActiveSlot].ActivateSlot();
        }
    }
    //Checks if skillholder is not null first:
    private bool CheckForSkillHolderReference()
    {
        if (skillHolder == null)
        {
            Debug.Log("Skill Holder reference not set");
            return false;
        }
        else return true;
    }
}
