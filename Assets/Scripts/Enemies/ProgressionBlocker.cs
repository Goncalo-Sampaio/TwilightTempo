using System.Collections.Generic;
using UnityEngine;

public class ProgressionBlocker : MonoBehaviour
{
    [SerializeField] private List<EnemyHealth> enemies = new();

    private void Start()
    {
        foreach (EnemyHealth enemy in enemies) enemy.SetProgressionBlocker(this);
    }
    public void RemoveEnemy(EnemyHealth enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
        {
            //This could be a courtine with animation clips:
            Destroy(gameObject);
        }
    }

    
}
