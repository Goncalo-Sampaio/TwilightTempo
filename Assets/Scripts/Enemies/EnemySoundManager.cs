using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class EnemySoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup defaultMixer;
    [SerializeField] private AudioMixerGroup reverbMixer;    

    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;     

    private EnemyReferences enemyReferences;
    private EnemyScriptableObject enemyData;

    private bool isRoaring = false;
    private void Awake()
    {
        enemyReferences = GetComponent<EnemyReferences>();
    }
    private void Start()
    {
        enemyData = enemyReferences.enemyData;
    }    
    public void PlaySpottedPlayerSoundEffect() =>  PlaySound(enemyData.spottedPlayer[(int)Random.Range(0, enemyData.spottedPlayer.Length)]);    
    public void PlayAttackSound() => PlaySound(enemyData.attackSFX[(int)Random.Range(0, enemyData.attackSFX.Length)]);
    public void PlayGettingHitSounds()
    {        
        PlaySound(enemyData.gettingHitSFX[(int)Random.Range(0, enemyData.gettingHitSFX.Length)]);        
        PlaySound(enemyData.hitSFX);
        PlaySound(enemyData.magicHitSFX);
    }
    public void PlayRoarSoundEffect()
    {
        if (!isRoaring) StartCoroutine(RoarRoutine());
    }
    public void PlayDeathSFX() => PlaySound(enemyData.deathSFX);
    private IEnumerator RoarRoutine()
    {
        isRoaring = true;
        audioSource2.outputAudioMixerGroup = reverbMixer;
        audioSource2.pitch = Random.Range(0.95f, 1.05f);
        audioSource2.PlayOneShot(enemyData.roarSFX[(int)Random.Range(0, enemyData.roarSFX.Length)]);
        yield return new WaitWhile(() => audioSource2.isPlaying);
        audioSource2.outputAudioMixerGroup = defaultMixer;
        isRoaring = false;
        yield return null;

    }
    private void PlaySound(AudioClip clip)
    {
        if (isRoaring) { return; }
        audioSource1.pitch = Random.Range(0.95f, 1.05f);
        audioSource1.PlayOneShot(clip);
    }
}
