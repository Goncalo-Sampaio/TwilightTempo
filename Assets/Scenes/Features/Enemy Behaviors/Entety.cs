using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected float health = 100f;
    protected bool dead = false;
    protected float timeTillDestroy = 1f;
    private void Awake()
    {
        dead = false;   
    }
    public virtual void Attack(Entity target,float damage) => target.TakeDamage(damage);    
    public virtual void TakeDamage(float damage)
    {
        if(!dead)
        {
            if (health - damage <= 0f)
            {
                dead = true;
                Die();
            }
            else health -= damage;
        }
    }
    protected virtual void Die() => StartCoroutine (DieCoroutine(timeTillDestroy));
    protected IEnumerator DieCoroutine (float time)
    {
        yield return new WaitForSeconds(time);
    }
}
