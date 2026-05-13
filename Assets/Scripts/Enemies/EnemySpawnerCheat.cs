using UnityEngine;

public class EnemySpawnerCheat : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private Transform spawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity);
        }
    }
}
