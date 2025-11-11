using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject finisherReady;
    [SerializeField]
    private Slider gauge;

    public void ActivateFinisher(bool activated)
    {
        finisherReady.SetActive(activated);
    }

    public void ChangeGauge(float gaugeValue)
    {
        gauge.value = gaugeValue;
    }
}
