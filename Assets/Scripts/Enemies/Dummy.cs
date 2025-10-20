using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField]
    private LayerMask playerDamageLayer;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((playerDamageLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Debug.Log("PlayerDamage");
            animator.SetTrigger("Hit");
        }
    }
}
