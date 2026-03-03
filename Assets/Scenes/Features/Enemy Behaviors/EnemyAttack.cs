using System;
using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Animator attackAnimator;
    private bool playerHit;
    public string collisionTag = "Player";
    [SerializeField] private GameObject sphereCollider;
    private WaitForSeconds attackerWindow;
    private WaitForSeconds startDelay;
    //reference to player health.

    //Honestly this whole ass class doesnt need to exist
    void Awake()
    {
        attackAnimator = GetComponent<Animator>();
        //Set default
        startDelay = new WaitForSeconds(.35f);
        sphereCollider = GetComponentInChildren<Collider>().transform.gameObject;
        SetAttackWindow(1f);
        
    }
    private void Start()
    {
        sphereCollider.SetActive(false);
    }
    public void SetAttackWindow(float attackWindow) => attackerWindow = new WaitForSeconds(attackWindow);
    public void AttackPlayer()
    {
        if (attackAnimator != null) attackAnimator.SetTrigger("Attack");
        if (!sphereCollider.activeSelf) StartCoroutine(AttackRot());
        //Also check for collisions and call TakeDamage() on player if contact

    }
    
    private IEnumerator AttackRot()
    {
        yield return startDelay;
        sphereCollider.SetActive(true);
        yield return attackerWindow;
        sphereCollider.SetActive(false);
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
                Debug.Log("Hit");
            }
        }
    }


}
