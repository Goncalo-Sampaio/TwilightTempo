using System;
using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private Animator attackAnimator;
    private EnemyReferences enemyReferences;
    private bool playerHit;
    private bool attacking = false;
    public string collisionTag = "Player";
    [SerializeField] private GameObject sphereCollider;
    [SerializeField] private float attackWindowStart = 0.45f;
    [SerializeField] private float attackWindowEnd = 0.65f;
    //reference to player health.
    private int attack01AnimHash = Animator.StringToHash("Base Layer.Attack01");

    //Honestly this whole ass class doesnt need to exist
    void Awake()
    {
        enemyReferences = GetComponentInParent<EnemyReferences>();
        //Only look for collider when no caster. Fix this later:
        if (!enemyReferences.isCaster) sphereCollider = GetComponentInChildren<Collider>().transform.gameObject;
    }
    private void Start()
    {
        sphereCollider.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (!attacking) return;
        if (CurrentAnimationCompletion() >= attackWindowStart && CurrentAnimationCompletion() <= attackWindowEnd) sphereCollider.SetActive(true);
        else sphereCollider.SetActive(false);
    }    
    public void Attacking() => attacking = true;
    public void StopAttacking() => attacking = false;
    
    private float CurrentAnimationCompletion()
    {
        AnimatorStateInfo stateInfo = attackAnimator.GetCurrentAnimatorStateInfo(0);
        //if animation clip is currently "Attack01"
        if (attack01AnimHash == stateInfo.fullPathHash)
        {
            return stateInfo.normalizedTime;
        }
        else return -1;      
    }
    //The collider here is the box that's being animated
    void OnCollisionEnter(Collision collision)
    {
        //if player already being hit ignore.
        // if not then detect player
        if (playerHit) return;
        
        else
        {   
            //use layer overrides to exclude self from detection
            //if player hit     
            if (collision.gameObject.tag == collisionTag)
            {
                collision.gameObject.GetComponent<PlayerHealth>().Damage();
                //call .TakeDamage() on its "Health" component
                
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if player already being hit ignore.
        // if not then detect player
        if (playerHit) return;

        else
        {
            //use layer overrides to exclude self from detection
            //if player hit     
            if (other.gameObject.tag == collisionTag)
            {
                other.gameObject.GetComponentInParent<PlayerHealth>().Damage();
                //call .TakeDamage() on its "Health" component                
            }
        }
    }


}
