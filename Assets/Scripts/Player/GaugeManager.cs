using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private PlayerStateManager playerStateManager;

    Dictionary<SkillAttunement, float> attunementCharges = new Dictionary<SkillAttunement, float>();

    private bool finisherReady = false;

    private void Start()
    {
        playerStateManager = GetComponentInParent<PlayerStateManager>();

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
            finisherReady = false;
            currentGauge = 0;
            foreach (var item in attunementCharges.Keys.ToList())
            {
                attunementCharges[item] = 0;
            }
            uiManager.ActivateFinisher(finisherReady);
            uiManager.ChangeGauge(currentGauge);
            StartCoroutine(ActivateFinisherCoroutine());
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
        if (attunementCharges[SkillAttunement.Music] > maxGauge / 2)
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
        }
    }

    private void ActivateForm(SkillAttunement attunement)
    {

    }

    private IEnumerator ActivateFinisherCoroutine()
    {
        playerStateManager.SetCurrentState(PlayerStates.Skill);
        animator.CrossFadeInFixedTime("Finisher", 0.25f, 0, 0.0f, 0.0f);
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
}
