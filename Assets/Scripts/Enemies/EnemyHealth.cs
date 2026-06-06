using DG.Tweening;
using DG.Tweening.Core.Easing;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

public class EnemyHealth : MonoBehaviour
{
    //change this to signaling
    //Make a base health class so i can stop copying code around 
    [SerializeField] private LayerMask playerDamageLayer;    
    private float currentHealth;
    private int maxHealth;
    private float knockBackResistance = 10f;
    public bool dead;

    
    
    private EnemyReferences enemyReferences;
    private ProgressionBlocker progressionBlocker;
    private Flash flash;    
    private bool gettingKnockBacked = false;
    [SerializeField][Tooltip("x out 10 chance of berserking after next hit when bellow 30% health")] private int chanceOfBerserking = 2;
    private float maxKnockBackTime;
    private float AfterDeathLingerTime;

    private UIManager uiManager;
    public bool invunerable = false;
    public void SetProgressionBlocker(ProgressionBlocker progressionBlocker) => this.progressionBlocker = progressionBlocker;
    CombatStats combatData;
    void Start()
    {
        enemyReferences = GetComponent<EnemyReferences>();
        combatData = FindAnyObjectByType<CombatDataManager>().combatData;
        knockBackResistance = enemyReferences.isCaster ? combatData.CasterKnockBackResistance : combatData.BrawlerKnockBackResistance;
        maxHealth = enemyReferences.isCaster ? combatData.CasterHealth : combatData.BrawlerHealth;
        maxKnockBackTime = combatData.MaxKnockBackTime;
        AfterDeathLingerTime = combatData.AfterDeathLingerTime;
            
        flash = enemyReferences.flash;
        currentHealth = maxHealth;
        uiManager = enemyReferences.uIManager;

        dead = false;
        if (LevelDataManager.Instance != null)
        {
            LevelDataManager.Instance.AddEnemy(this);            
        }
        else Debug.LogWarning("LevelDataManager is missing - Add one to the scene");
    }
    //Just Damage
    public void Damage(float damage)
    {
        if (invunerable) return;
        Debug.Log($"{gameObject.name} got damaged");
        enemyReferences.enemySoundManager.PlayGettingHitSounds();

        currentHealth -= damage;


        if (RollTheDice())
        {
            if (currentHealth < maxHealth * .3f && !enemyReferences.enemyBrain.isBerserk)
            {
                if (!enemyReferences.isCaster) enemyReferences.enemyBrain.Berserk();
            }
        }
        if (currentHealth <= 0)
        {
            if (progressionBlocker != null)
            {
                progressionBlocker.RemoveEnemy(this);
            }

            if (!dead)
            {
                uiManager.UpdateEnemyHealth(false, 0, 0);
                StartCoroutine(DeathRot());                
            }

        }

        //VISUAL FEEDBACK:
        //Flash once
        if (!dead)
        {
            uiManager.UpdateEnemyHealth(true, maxHealth, currentHealth);
            enemyReferences.enemyAnimator.HitStop(10);
            enemyReferences.enemyBrain.GotHit();
        }

    }
    //this is for when the player position is better for direction like the meele attacks
    public void Damage(float damage, float knockBackForce)
    {

        Vector3 KnockBackDirection = (gameObject.transform.position - enemyReferences.playerRef.position).normalized;
        Vector3 KnockBackDirectionFlat = new Vector3(KnockBackDirection.x, gameObject.transform.position.y, KnockBackDirection.z);
        Damage(damage, KnockBackDirectionFlat * knockBackForce * 5);
    }
    public void Damage (float damage, float knockBackForce,Vector3 knockBackOriginPos)
    {
        Vector3 KnockBackDirection = (gameObject.transform.position - knockBackOriginPos).normalized;
        Vector3 KnockBackDirectionFlat = new Vector3(KnockBackDirection.x, gameObject.transform.position.y, KnockBackDirection.z);
        Damage(damage, KnockBackDirectionFlat * knockBackForce*5);
    }
    //With KnockBack
    public void Damage(float damage,Vector3 force)
    {
        if (invunerable) return;
        enemyReferences.enemySoundManager.PlayGettingHitSounds();
        currentHealth -= damage;
        Vector3 forceAfterKnockBackNegation = force - (force * knockBackResistance / 100 );
        ApllyKnockBack(forceAfterKnockBackNegation);
        if(RollTheDice())
        {
            if (currentHealth < maxHealth * .3f && !enemyReferences.enemyBrain.isBerserk)
            {
                if (!enemyReferences.isCaster) enemyReferences.enemyBrain.Berserk();
            }
        }

        if (currentHealth <= 0)
        {
            if (progressionBlocker != null)
            {
                progressionBlocker.RemoveEnemy(this);
            }

            if (!dead)
            {
                StartCoroutine(DeathRot());
                
            }
            
        }
        //VISUAL FEEDBACK:
        //Flash once
        if (!dead)
        {
            enemyReferences.enemyAnimator.HitStop(10);
            enemyReferences.enemyBrain.GotHit();
        }
        //flash.FlashForXIterations(1);

        //transform.DOShakePosition(0.2f, 0.1f, 10);

    }    
    
    [Button]
    public void ApllyKnockBack() => ApllyKnockBack(10f * -transform.forward + transform.up);
    public void ApllyKnockBack(Vector3 force) => StartCoroutine("ApplyKnockBackRot", force);
    //Change this to just use normal ass phisics
    private IEnumerator ApplyKnockBackRot(Vector3 force)
    {
        gettingKnockBacked = true;

        yield return null; //wait one frame to make sure all courotines are stopped
        //Only call agent.Stop if:
        //  agent is active
        //  agent is on NavMesh;
        //  agent isint' already stopped
        
        enemyReferences.enemyNavigation.StopNow(true);
        enemyReferences.enemyNavigation.ToggleEnableAgent(false); //disable agent
        
        enemyReferences.rb.linearVelocity = Vector3.zero;
        enemyReferences.rb.angularVelocity = Vector3.zero;
        enemyReferences.rb.AddForce(force, ForceMode.Impulse);
        
        //only exit after the fixedUpdate frame is passed. To make sure the force is applied
        yield return new WaitForFixedUpdate();
        float knockBackTime = Time.time;

        yield return new WaitUntil(() => enemyReferences.rb.linearVelocity.magnitude < 0.05f || Time.time > knockBackTime + maxKnockBackTime); //wait until it stops moving.

        
        yield return new WaitForSeconds(0.25f); //stun frames //consider adding a flash here

        //now reset:        
        enemyReferences.rb.linearVelocity = Vector3.zero;
        enemyReferences.rb.angularVelocity = Vector3.zero;
        //snap agent back to navmesh
        //enemyReferences.enemyNavigation.Warp(transform.position);
        //THEN AND ONLY THEN
        //enable the agent
        enemyReferences.enemyNavigation.ToggleEnableAgent(true); //enable agent
        enemyReferences.enemyNavigation.ToggleAgentStopped(false); //start agent navmesh

        gettingKnockBacked = false;
    }
    private IEnumerator ApplyKnockBackRot2(Vector3 force)
    {
        gettingKnockBacked = true;

        yield return null; //wait one frame to make sure all courotines are stopped
        //Only call agent.Stop if:
        //  agent is active
        //  agent is on NavMesh;
        //  agent isint' already stopped

        enemyReferences.enemyNavigation.StopNow(true);
        enemyReferences.enemyNavigation.ToggleEnableAgent(false); //disable agent

        enemyReferences.enemyNavigation.moving = false;
        enemyReferences.rb.AddForce(force, ForceMode.Impulse);


        //only exit after the fixedUpdate frame is passed. To make sure the force is applied
        yield return new WaitForFixedUpdate();
        float knockBackTime = Time.time;

        yield return new WaitUntil(() => enemyReferences.rb.linearVelocity.magnitude < 0.1f || Time.time > knockBackTime + maxKnockBackTime); //wait until it stops moving.


        yield return new WaitForSeconds(0.25f); //stun frames //consider adding a flash here

        //now reset:
        enemyReferences.rb.linearVelocity = Vector3.zero;
        enemyReferences.rb.angularVelocity = Vector3.zero;
        enemyReferences.rb.useGravity = false;
        enemyReferences.rb.isKinematic = true;

        //snap agent back to navmesh
        enemyReferences.enemyNavigation.Warp(transform.position);
        //THEN AND ONLY THEN
        //enable the agent
        enemyReferences.enemyNavigation.ToggleEnableAgent(true); //enable agent
        enemyReferences.enemyNavigation.ToggleAgentStopped(false); //start agent navmesh

        gettingKnockBacked = false;
    }


    private IEnumerator DeathRot()
    {
        dead = true;
        enemyReferences.enemySoundManager.PlayDeathSFX();
        enemyReferences.enemyNavigation.StopNow(true);        
        enemyReferences.enemyBrain.Die();
        LevelDataManager.Instance.RemoveEnemy(this);
        yield return new WaitForSeconds(AfterDeathLingerTime);        
        //Instead of destroy just leave the bodies:
        //Destroy(this.gameObject);

    }    
    private bool RollTheDice()
    {
        return (chanceOfBerserking >= Random.Range(0, 10)) ;
    }

}
