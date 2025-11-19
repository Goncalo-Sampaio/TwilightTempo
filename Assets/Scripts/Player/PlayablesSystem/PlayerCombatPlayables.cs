using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombatPlayables : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimationClip[] attacks;
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
                Debug.Log("Exit attack");
                exitAttack = false;
                ExitAttack();
            }
        }
    }

    private void Attack()
    {
        if ((Time.time - lastComboEnd > 0.5f) && (comboCounter < attacks.Count()))
        {
            Debug.Log("Attack");
            playerStateManagerPlayables.SetCurrentState(PlayerStates.Attacking);

            playerStateManagerPlayables.Attack(attacks[comboCounter]);
            StartAttack();
        }
    }

    private void StartAttack()
    {
        exitAttackTimer = attacks[comboCounter].length * attackExitTimers[comboCounter];
        Debug.Log(exitAttackTimer);
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
        Debug.Log("ContinueCombo");
        playerStateManagerPlayables.Attack(attacks[comboCounter]);
        StartAttack();
    }

    private void EndCombo()
    {
        Debug.Log("EndingCombo");
        comboCounter = 0;
        lastComboEnd = Time.time;
        playerStateManagerPlayables.ResetState();
    }

    public void EnableCombo()
    {
        Debug.Log("EnableCombo");
        canCombo = true;
    }

    public void DisableCombo()
    {
        Debug.Log("DisableCombo");
        canCombo = false;
    }
}
