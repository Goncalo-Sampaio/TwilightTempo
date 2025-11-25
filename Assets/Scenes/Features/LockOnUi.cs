
using UnityEngine;

public class LockOnUi : MonoBehaviour
{

    [SerializeField] private Transform lockOnPivot;
    public GameObject currentLockOnTarget;
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void OnEnable()
    {
        if(animator != null) animator.Play("LockEnable");
    }
    void LateUpdate()
    {
        transform.position = currentLockOnTarget.transform.position;
        lockOnPivot.transform.LookAt(Camera.main.transform.position, Vector3.up);
    }
    public void SetLockOnTarget(GameObject target) => currentLockOnTarget = target;
    
}
