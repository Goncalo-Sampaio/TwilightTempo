using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "Enemies/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    //store enemy specific sound effects
    //public int Health;
    //public float MoveSpeed;
    //public float TurnSpeed;
    public EnemyType enemyType;
    [Header("SFX")]
    public AudioClip[] attackSFX;
    public AudioClip[] gettingHitSFX;
    public AudioClip[] spottedPlayer;
    public AudioClip hitSFX;
    public AudioClip magicHitSFX;
    public AudioClip deathSFX;
    [Header("Brawler Specific")]
    public AudioClip[] roarSFX;
}
public enum EnemyType { Caster, Brawler}
