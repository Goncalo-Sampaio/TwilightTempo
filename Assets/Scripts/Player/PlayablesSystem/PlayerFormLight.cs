using System.Linq;
using UnityEngine;

public class PlayerFormLight : MonoBehaviour
{
    [SerializeField]
    private AnimationClip[] attacks;
    [SerializeField]
    private ParticleSystem[] attackVFX;
    [SerializeField]
    private float[] attackExitTimers;
    private float lastComboEnd;
    private int comboCounter;

    private PlayerStateManagerPlayables playerStateManagerPlayables;
    private PlayerStates currentState;

    private bool canCombo = false;
    private bool continueCombo = false;

    private bool exitAttack = false;
    private float exitAttackTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateManagerPlayables = GetComponent<PlayerStateManagerPlayables>();
        currentState = playerStateManagerPlayables.CurrentState;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && currentState <= PlayerStates.Attacking)
        {
            if (canCombo)
            {
                canCombo = false;
                continueCombo = true;
            }

            if (comboCounter == 0)
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
    }

    private void Attack()
    {
        if ((Time.time - lastComboEnd > 0.5f) && (comboCounter < attacks.Count()))
        {
            playerStateManagerPlayables.SetCurrentState(PlayerStates.Attacking);

            playerStateManagerPlayables.Attack(attacks[comboCounter]);
            StartAttack();
        }
    }

    private void StartAttack()
    {
        exitAttackTimer = attacks[comboCounter].length * attackExitTimers[comboCounter];
        exitAttack = true;
        comboCounter++;
    }

    public void ExitAttack()
    {
        DisableCombo();

        if (comboCounter == attacks.Length)
        {
            continueCombo = false;
        }

        if (continueCombo)
        {
            continueCombo = false;
            ContinueCombo();
        }
        else
        {
            EndCombo();
        }
    }

    private void ContinueCombo()
    {
        playerStateManagerPlayables.Attack(attacks[comboCounter]);
        StartAttack();
    }

    private void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
        playerStateManagerPlayables.ResetState();
    }

    public void EnableCombo()
    {
        PlayVFX(comboCounter - 1);
        canCombo = true;
    }

    public void DisableCombo()
    {
        canCombo = false;
    }

    private void PlayVFX(int counter)
    {
        if (counter == 0)
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
