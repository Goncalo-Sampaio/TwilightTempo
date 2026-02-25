using UnityEngine;

public class EnemySpawnerOnDestroyEnemy : MonoBehaviour
{
    [SerializeField]
    public EnemySpawner enemySpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        enemySpawner.Remove(this.gameObject);
    }
}
