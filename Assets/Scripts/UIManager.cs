using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{    
    [SerializeField]
    private GameObject finisherReady;
    [Header("Referenceable UI elements:")]
    [SerializeField] private Slider gauge;
    [SerializeField] private GameObject skillTimer1;
    [SerializeField] private GameObject skillTimer2;
    [SerializeField] private GameObject skillTimer3;
    [SerializeField] private GameObject skillTimer4;
    [SerializeField] private GameObject skillTimer5;
    [SerializeField] private GameObject skillTimer6;
    public GameObject skillHolder;
    public Slider healthUI;
    
    public void ActivateFinisher(bool activated)
    {
        finisherReady.SetActive(activated);
    }

    public void ChangeGauge(float gaugeValue)
    {
        gauge.value = gaugeValue;
    }
    private void Start()
    {
        if (LevelDataManager.Instance != null)
        {            
            LevelDataManager.Instance.RegisterCanvas(this);
            
        }
        else Debug.LogWarning("LevelDataManager is missing - Add one to the scene"); 
    }    
    
    public GameObject GetSkillVisual(int skillNumber)
    {
        if (skillNumber == 1) return skillTimer1;
        else if (skillNumber == 2) return skillTimer2;
        else if (skillNumber == 3) return skillTimer3;
        else if (skillNumber == 4) return skillTimer4;
        else if (skillNumber == 5) return skillTimer5;
        else if (skillNumber == 6) return skillTimer6;
        else
        {
            Debug.Log($"No Skills with {skillNumber} skill Number");
            return null;
        }
    }
}
