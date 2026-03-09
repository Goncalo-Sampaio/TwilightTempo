using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //private List<EnemyHealth> enemies = new List<EnemyHealth>();
    public static EnemyManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }
    [SerializeField] public PlayerHealth playerHealthRef;    
    
}
