using System.Linq;
using UnityEngine;

public class PlayerCombatPlayables : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AudioClip attackWhooshSFX;
    [SerializeField]
    private AudioClip sparklesSFX;

    [SerializeField]
    private AnimationClip[] attacks;
    [SerializeField]
    private float attackSpeed = 2;
    [SerializeField]
    private ParticleSystem[] attackVFX;
    [SerializeField]
    private float[] attackExitTimers;
    [SerializeField]
    private float comboWindowTime = 0.4f;
    private float comboTimer;
    private float lastComboEnd;
    private int comboCounter;

    private PlayerStateManagerPlayables playerStateManagerPlayables;
    private MovementPlayables movementPlayables;
    private PlayerStates currentState;
    private AudioSource audioSource;
    private ThirdPersonCam thirdPersonCam;

    private bool canCombo = false;
    private bool continueCombo = false;

    private bool exitAttack = false;
    private float exitAttackTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateManagerPlayables = GetComponent<PlayerStateManagerPlayables>();
        movementPlayables = GetComponent<MovementPlayables>();
        currentState = playerStateManagerPlayables.CurrentState;
        audioSource = GetComponent<AudioSource>();
        thirdPersonCam = GetComponentInChildren<ThirdPersonCam>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && currentState <= PlayerStates.Attacking)
        {
            if (comboCounter == 0 || continueCombo)
            {
                Attack();
            }
        }
    }

    private void FixedUpdate()
    {
        currentState = playerStateManagerPlayables.CurrentState;
        exitAttackTimer -= Time.fixedDeltaTime;

        if (exitAttackTimer <= 0f)
        {
            exitAttackTimer = 0f;

            if (exitAttack)
            {
                exitAttack = false;
                ExitAttack();
            }
        }

        comboTimer -= Time.fixedDeltaTime;

        if (comboTimer <= 0f)
        {
            comboTimer = 0f;

            if (canCombo)
            {
                canCombo = false;
                continueCombo = false;
                comboCounter = 0;
            }
        }
    }

    private void Attack()
    {
        if (continueCombo)
        {
            continueCombo = false;
            playerStateManagerPlayables.SetCurrentState(PlayerStates.Attacking);
            thirdPersonCam.Attacking = true;

            ContinueCombo();
        }
        else if ((Time.time - lastComboEnd > 0.1f) && (comboCounter < attacks.Count()))
        {
            playerStateManagerPlayables.SetCurrentState(PlayerStates.Attacking);
            thirdPersonCam.Attacking = true;

            playerStateManagerPlayables.Attack(attacks[comboCounter], attackSpeed);
            movementPlayables.AttackBoost();
            StartAttack();
        }

    }

    private void StartAttack()
    {
        audioSource.pitch = Random.Range(0.7f, 0.9f);
        audioSource.PlayOneShot(attackWhooshSFX);
        audioSource.pitch = Random.Range(0.98f, 1.02f);
        audioSource.PlayOneShot(sparklesSFX);
        comboTimer = comboWindowTime + attacks[comboCounter].length / attackSpeed;
        exitAttackTimer = attacks[comboCounter].length / attackSpeed * attackExitTimers[comboCounter];
        exitAttack = true;
        comboCounter++;
    }

    public void ExitAttack()
    {
        if (comboCounter == attacks.Length)
        {
            DisableCombo();
        }
        else
        {
            canCombo = true;
            continueCombo = true;
        }

        playerStateManagerPlayables.ResetState();
        thirdPersonCam.Attacking = false;

        lastComboEnd = Time.time;
    }

    private void ExitCombo()
    {
        DisableCombo();
    }

    private void ContinueCombo()
    {
        playerStateManagerPlayables.Attack(attacks[comboCounter], attackSpeed);

        movementPlayables.AttackBoost();
        StartAttack();
    }

    private void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }

    public void EnableCombo()
    {
        PlayVFX(comboCounter - 1);
    }

    public void DisableCombo()
    {
        canCombo = false;
        comboCounter = 0;
    }

    private void PlayVFX(int counter)
    {
        if (counter  == 0)
        {
            attackVFX[0].Play();
        }
        else if (counter == 1)
        {
            attackVFX[1].Play();
        }
        else if (counter == 2)
        {
            attackVFX[2].Play();
            attackVFX[3].Play();
        }
    }
}
