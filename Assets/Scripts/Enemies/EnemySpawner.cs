using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Transform[] spawnPositions;

    [SerializeField]
    private List<GameObject> enemies = new List<GameObject>();

    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Remove(GameObject enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        foreach (Transform t in spawnPositions)
        {
            GameObject enemy = Instantiate(enemyPrefab, t.transform.position, Quaternion.identity);
            enemyPrefab.GetComponent<EnemySpawnerOnDestroyEnemy>().enemySpawner = this;
            enemyPrefab.GetComponent<EnemyBrain>().Player = player;
            enemyPrefab.GetComponent<EnemyHealth>().player = player;
            enemies.Add(enemy);
        }
    }
}
