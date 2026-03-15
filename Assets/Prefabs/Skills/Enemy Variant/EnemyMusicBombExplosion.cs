using UnityEngine;

public class EnemyMusicBombExplosion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("SelfDestruct", 1);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
