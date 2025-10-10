using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnimator;

    public bool CanAct { get; private set; }
    public PlayerStates CurrentState { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentState = PlayerStates.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(CurrentState);
    }

    public void SetCurrentState(PlayerStates state)
    {
        if (state > CurrentState)
        {
            CurrentState = state;

            if (state == PlayerStates.Attacking)
            {
                playerAnimator.SetFloat("Velocity", 0);
            }

            if (state == PlayerStates.Moving)
            {
                playerAnimator.SetBool("Running", true);
            }
        }
    }

    public void ResetState()
    {
        CurrentState = PlayerStates.Idle;
        playerAnimator.SetBool("Running", false);
    }

    public void SetVelocity(float velocity)
    {
        playerAnimator.SetFloat("Velocity", velocity);
    }
}
