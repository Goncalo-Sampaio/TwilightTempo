using System.Collections.Generic;
using UnityEngine;

public class ProgressionBlocker : MonoBehaviour
{
    [SerializeField] private List<EnemyHealth> enemies = new();

    [SerializeField]
    private LayerMask playerLayer;

    private bool spawnTriggered = false;

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

    private void OnTriggerEnter(Collider other)
    {
        if ((playerLayer.value & (1 << other.transform.gameObject.layer)) > 0 && !spawnTriggered)
        {
            spawnTriggered = true;
            foreach (EnemyHealth enemy in enemies)
            {
                enemy.gameObject.GetComponentInChildren<EnemyAnimator>().EnableSpawn();
            }
        }
    }
}
