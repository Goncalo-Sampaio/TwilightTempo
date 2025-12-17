using System;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Animator attackAnimator;
    private bool playerHit;
    public string collisionTag = "Player";
    //reference to player health.
    void Awake()
    {
        attackAnimator = GetComponent<Animator>();
    }
    public void AttackPlayer()
    {
        //Also check for collisions and call TakeDamage() on player if contact
        attackAnimator.SetTrigger("Attack");
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
    

}
