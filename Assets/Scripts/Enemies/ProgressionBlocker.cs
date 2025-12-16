using System.Collections.Generic;
using UnityEngine;

public class ProgressionBlocker : MonoBehaviour
{
    private List<EnemyHealth> enemies = new();

    public void RemoveEnemy(EnemyHealth enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    public void RegisterEnemy(EnemyHealth enemy)
    {
        enemies.Add(enemy);
    }
}
