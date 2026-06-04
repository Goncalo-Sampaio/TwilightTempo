using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class EnemySoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup defaultMixer;
    [SerializeField] private AudioMixerGroup reverbMixer;
    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private AudioClip roarSFX;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip magicHitSFX;
    private AudioSource audioSource;
    private bool isRoaring = false;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayRoarSoundEffect()
    {
        if (!isRoaring) StartCoroutine(RoarRoutine());
    }
    public void PlayAttackSound()
    {
        if (isRoaring) { return; }
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(attackSFX);
    }
    public void PlayGettingHitSounds()
    {
        if (isRoaring) { return; }
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(hitSFX);
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(magicHitSFX);
    }
    private void Update()
    {        
        if (Input.GetKeyDown(KeyCode.M)) PlayRoarSoundEffect();
    }
    private IEnumerator RoarRoutine()
    {
        isRoaring = true;
        audioSource.outputAudioMixerGroup = reverbMixer;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(roarSFX);
        yield return new WaitWhile(() => audioSource.isPlaying);
        audioSource.outputAudioMixerGroup = defaultMixer;
        isRoaring = false;
        yield return null;


    }
}
