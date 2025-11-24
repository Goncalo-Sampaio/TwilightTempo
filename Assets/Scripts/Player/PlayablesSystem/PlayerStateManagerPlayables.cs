using UnityEngine;

public class PlayerStateManagerPlayables : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnimator;
    private AnimationSystem animationSystem;

    [Header("Animation Clips")]
    [SerializeField]
    private AnimationClip idleAnimation;
    [SerializeField]
    private AnimationClip runAnimation;



    public bool CanAct { get; private set; }
    public PlayerStates CurrentState { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentState = PlayerStates.Idle;
        animationSystem = new AnimationSystem(playerAnimator, idleAnimation, runAnimation);
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
            Debug.Log(state.ToString());
            CurrentState = state;

            if (state == PlayerStates.Attacking)
            {
                //playerAnimator.SetFloat("Velocity", 0);
            }

            if (state == PlayerStates.Moving)
            {
                //playerAnimator.SetBool("Running", true);
            }
        }
    }

    public void ResetState()
    {
        Debug.Log("State reset");
        CurrentState = PlayerStates.Idle;
        //playerAnimator.SetBool("Running", false);
    }

    public void SetVelocity(float velocity, float maxSpeed, bool grounded)
    {
        //playerAnimator.SetFloat("Velocity", velocity);
        animationSystem.UpdateLocomotion(velocity, maxSpeed, grounded);
    }

    public void Attack(AnimationClip attackClip)
    {
        animationSystem.PlayOneShot(attackClip);
    }

    public void Jump(AnimationClip jumpClip)
    {
        animationSystem.PlayJump(jumpClip);
    }
}
