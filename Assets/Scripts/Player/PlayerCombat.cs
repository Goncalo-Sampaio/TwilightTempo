using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private List<AttacksSO> combo;
    private float lastClickedTime;
    private float lastComboEnd;
    private int comboCounter;

    private PlayerStateManager playerStateManager;
    private PlayerStates currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateManager = GetComponent<PlayerStateManager>();
        currentState = playerStateManager.CurrentState;
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
        if (Time.time - lastComboEnd > 0.5f && comboCounter < combo.Count)
        {
            playerStateManager.SetCurrentState(PlayerStates.Attacking);
            CancelInvoke("EndCombo");

            if (Time.time - lastClickedTime >= 0.6f)
            {
                animator.runtimeAnimatorController = combo[comboCounter].AnimatorOV;
                //animator.Play("Attack", 0, 0);
                animator.CrossFadeInFixedTime("Attack", 0.25f, 0, 0.0f, 0.0f);
                //activate weapon and/or damage
                //also play knockbacks or visual effects here
                comboCounter++;
                lastClickedTime = Time.time;

                if (comboCounter >= combo.Count)
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
            Invoke("EndCombo", 0.5f);
        }
    }

    private void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
        playerStateManager.ResetState();
    }
}
