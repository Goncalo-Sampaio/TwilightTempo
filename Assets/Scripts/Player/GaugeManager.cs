using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class GaugeManager : MonoBehaviour
{
    [SerializeField]
    private float maxGauge;
    [SerializeField]
    private float currentGauge;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private GameObject finisher;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimationClip finisherAnimation;
    [SerializeField]
    private PlayerCinematics playerCinematics;
    [SerializeField]
    private Material finisherReadyMaterial;
    [SerializeField]
    private float maxPower = 3f;
    [SerializeField]
    private float maxIntensity = 10f;
    [SerializeField]
    private float finisherReadyCounter = 0f;
    [SerializeField]
    private AnimationCurve finisherReadyPowerCurve;
    [SerializeField]
    private AnimationCurve finisherReadyIntensityCurve;

    private PlayerStateManagerPlayables playerStateManager;

    private AudioSource audioSource;


    Dictionary<SkillAttunement, float> attunementCharges = new Dictionary<SkillAttunement, float>();

    private bool finisherReady = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerStateManager = GetComponentInParent<PlayerStateManagerPlayables>();

        attunementCharges.Add(SkillAttunement.None, 0f);
        attunementCharges.Add(SkillAttunement.Music, 0f);
        attunementCharges.Add(SkillAttunement.Dance, 0f);
        attunementCharges.Add(SkillAttunement.Light, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && finisherReady)
        {
            ResetGauge();
            StartCoroutine(ActivateFinisherCoroutine());
        }
    }

    private void FixedUpdate()
    {
        if (finisherReady && finisherReadyCounter <= 2f)
        {
            finisherReadyCounter += Time.fixedDeltaTime;

            finisherReadyMaterial.SetFloat("_VignetePower", maxPower * finisherReadyPowerCurve.Evaluate(finisherReadyCounter/2));
            finisherReadyMaterial.SetFloat("_VigneteIntensity", maxIntensity * finisherReadyIntensityCurve.Evaluate(finisherReadyCounter/2));
        }
    }

    public void IncreaseGauge(float increase, SkillAttunement attunement)
    {
        if (finisherReady || currentGauge >= maxGauge)
        {
            return;
        }

        attunementCharges[attunement] += increase;
        currentGauge += increase;
        if (currentGauge >= maxGauge)
        {
            currentGauge = maxGauge;
            CheckFormToActivate();
        }
        uiManager.ChangeGauge(currentGauge);
    }

    private void CheckFormToActivate()
    {
        /*if (attunementCharges[SkillAttunement.Music] > maxGauge / 2)
        {
            ActivateForm(SkillAttunement.Music);
        }
        else if (attunementCharges[SkillAttunement.Dance] > maxGauge / 2)
        {
            ActivateForm(SkillAttunement.Dance);
        }
        else if (attunementCharges[SkillAttunement.Light] > maxGauge / 2)
        {
            ActivateForm(SkillAttunement.Light);
        }
        else
        {
            finisherReady = true;
            uiManager.ActivateFinisher(finisherReady);
        }*/

        finisherReady = true;
        finisherReadyCounter = 0f;
        audioSource.Play();
        uiManager.ActivateFinisher(finisherReady);
    }

    private void ActivateForm(SkillAttunement attunement)
    {
        ResetGauge();
        Debug.LogWarning("Light Form Active");
    }

    private IEnumerator ActivateFinisherCoroutine()
    {
        playerStateManager.SetCurrentState(PlayerStates.Finisher);
        playerStateManager.Attack(finisherAnimation);
        playerCinematics.ActivateFinisher();
        yield return new WaitForSeconds(1.4f);

        for (int i = 0; i < finisher.transform.childCount; i++)
        {
            finisher.transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
        playerStateManager.ResetState();

        yield return new WaitForSeconds(1f);
        for (int i = 0; i < finisher.transform.childCount; i++)
        {
            finisher.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void ResetGauge()
    {
        finisherReady = false;
        currentGauge = 0;
        foreach (var item in attunementCharges.Keys.ToList())
        {
            attunementCharges[item] = 0;
        }
        uiManager.ActivateFinisher(finisherReady);
        uiManager.ChangeGauge(currentGauge);
    }
}
