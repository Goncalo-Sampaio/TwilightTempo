using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombatPlayables : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimationClip[] attacks;
    private float lastClickedTime;
    private float lastComboEnd;
    private int comboCounter;

    private PlayerStateManagerPlayables playerStateManagerPlayables;
    private PlayerStates currentState;

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
            Attack();
        }

        ExitAttack();
    }

    private void Attack()
    {
        if ((Time.time - lastComboEnd > 0.5f) && (comboCounter < attacks.Count()))
        {
            Debug.Log("Attack");
            Debug.Log(currentState);
            playerStateManagerPlayables.SetCurrentState(PlayerStates.Attacking);
            CancelInvoke(nameof(EndCombo));

            if (Time.time - lastClickedTime >= 0.6f)
            {
                playerStateManagerPlayables.Attack(attacks[comboCounter]);
                comboCounter++;
                lastClickedTime = Time.time;

                if (comboCounter >= attacks.Count())
                {
                    //comboCounter = 0;
                }
            }
        }
    }

    private void ExitAttack()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke(nameof(EndCombo), 0.5f);
        }
    }

    private void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
        playerStateManagerPlayables.ResetState();
    }
}
