using UnityEngine;

public class CombatDataManager : MonoBehaviour
{
    public static CombatDataManager Instance;
    public CombatStats combatData;
    public void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
