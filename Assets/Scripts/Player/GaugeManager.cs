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

    Dictionary<SkillAttunement, float> attunementCharges = new Dictionary<SkillAttunement, float>();

    private bool finisherReady = false;

    private void Start()
    {
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
}
