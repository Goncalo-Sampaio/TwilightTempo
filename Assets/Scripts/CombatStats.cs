using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatStats", menuName = "GameCombatData/CombatStats")]
public class CombatStats : ScriptableObject
{

    [Header("Player Stats")]
    //public int PlayerHealth;
    //public float PlayerSpeed;
    public PlayerAttack BaseAttack;
    public PlayerAttack Dash;    
    public PlayerAttack LightDash;
    public PlayerAttack MusicBomb;
    public PlayerAttack LuminousLazer;    
    public PlayerAttack Finisher;

    [Header("Enemy Brawler Stats")]
    public int BrawlerHealth;
    public float BrawlerMovementSpeed;
    public int BrawlerAttackDamage;
    public float BrawlerAttackSpeed;
    public float BrawlerKnockBackResistance;
    public float BerserkDamageMultiplier;
    public float BerserkKnockBackResistanceMultiplier;

    [Header("Enemy Caster Stats")]
    public int CasterHealth;
    public float CasterMovementSpeed;
    public int CasterAttackDamage;
    public float CasterAttackSpeed;
    public float CasterKnockBackResistance;


    [Header("Universal Enemy Stats")]
    public float MaxKnockBackTime;
    public float AfterDeathLingerTime;



}
[Serializable]
public struct PlayerAttack
{
    public int Damage;
    public float KnockbackForce;
    public float StunTime;
    public float CoolDownTime;
    public float GuageIncrease;
    public PlayerAttack(int Damage, float KnockbackForce, float StunTime = 0f, float CoolDownTime = 0f, float GuageIncrease = 5f)
    {
        this.Damage = Damage;
        this.KnockbackForce = KnockbackForce;
        this.StunTime = StunTime;
        this.CoolDownTime = CoolDownTime;
        this.GuageIncrease = GuageIncrease;
    }
    
}
